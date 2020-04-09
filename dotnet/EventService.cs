using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Sabio.Data;
using Sabio.Models.Requests;
using Sabio.Models.Requests.NewFolder;
using Newtonsoft.Json;
using Sabio.Models.Requests.Files;
using Sabio.Models.Requests.Event;
using Sabio.Models.Requests.Venue;
using Sabio.Models.Requests.EventParticipant;

namespace Sabio.Services
{
    public class EventService : IEventService
    {

        IDataProvider _data = null;
        public EventService(IDataProvider data)
        {
            _data = data;
        }
        
        public Paged<Event> Paginate(int pageIndex, int pageSize)
        {
            Paged<Event> pagedResult = null;
            List<Event> events = null;
            string procName = "[dbo].[Events_SelectAll_V5]";
            int totalCount = 0;

            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@PageIndex", pageIndex);
                parameterCollection.AddWithValue("@PageSize", pageSize);
            },
            singleRecordMapper: delegate (IDataReader reader, short set)
            {
                Event e = EventMapper(reader);
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(48);
                }
                if (events == null)
                {
                    events = new List<Event>();
                }
                events.Add(e);
            }
                );
            if (events != null)
            {
                pagedResult = new Paged<Event>(events, pageIndex, pageSize, totalCount);
            }
            return pagedResult;  
        }

        public Paged<Event> Search(string search, int pageIndex, int pageSize)
        {
            Paged<Event> pagedResult = null;
            List<Event> events = null;
            string procName = "[dbo].[Events_Search_V2]";
            int totalCount = 0;

            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@Search", search);
                parameterCollection.AddWithValue("@PageIndex", pageIndex);
                parameterCollection.AddWithValue("@PageSize", pageSize);
            },
            singleRecordMapper: delegate (IDataReader reader, short set)
            {
                Event e = EventMapper(reader);
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(48);
                }
                if (events == null)
                {
                    events = new List<Event>();
                }
                events.Add(e);
            }
                );
            if (events != null)
            {
                pagedResult = new Paged<Event>(events, pageIndex, pageSize, totalCount);
            }
            return pagedResult;
        }

        

        public Event Get(int id)
        {
            string procName = "[dbo].[Events_Select_ById_V5]";
            Event e = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                e = EventMapper(reader);
            });
            return e;
        }

    

        public List<int> GetByUserId(int userId)
        {

            List<int> eps = null;
            string procName = "[dbo].[EventParticipants_Select_ByUserId]";

            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@UserId", userId);
            }, singleRecordMapper: delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;

               int eventId = reader.GetSafeInt32(startingIndex++);

                if (eps == null)
                {
                    eps = new List<int>();
                }
                eps.Add(eventId);
            });

            return eps;
        }

        public int Add(EventAddMultiStep model, int userId)
        {

            int id = 0;
       
            string procName = "[dbo].[Events_Insert_V3]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                DataTable dtEventFiles, dtVenueFiles;
                DataTableMapper(model, out dtEventFiles, out dtVenueFiles);

                EventParams(model, userId, col, dtEventFiles, dtVenueFiles);
                SqlParameter idOut = new SqlParameter("@EventId", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;
                col.Add(idOut);

            },
            returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                object oId = returnCollection["@EventId"].Value;
                int.TryParse(oId.ToString(), out id);

            });

            return id;
        }

        public void AddParticipant(EventParticipantAddRequest model, int userId)
        {

            string procName = "[dbo].[EventParticipants_Insert]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@EventId", model.EventId);
                col.AddWithValue("@UserId", userId);
                col.AddWithValue("@ParticipantTypeId", model.ParticipantTypeId);

            },
            returnParameters: null);

        }

        public void Update(EventUpdateMultiStep model, int userId)
        {
            string procname = "[dbo].[Events_Update_V3]";
            _data.ExecuteNonQuery(procname, inputParamMapper: delegate (SqlParameterCollection col)
            {
                DataTable dtEventFiles, dtVenueFiles;
                DataTableMapper(model, out dtEventFiles, out dtVenueFiles);
                EventParams(model, userId, col, dtEventFiles, dtVenueFiles);
                col.AddWithValue("@EventId", model.Id);
            },
            returnParameters: null);
        }

        public void UpdateStatus(UpdateStatusRequest model, int userId)
        {
            string procname = "[dbo].[Events_Update_V4]";
            _data.ExecuteNonQuery(procname, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("Id", model.Id);
                col.AddWithValue("@EventStatusId", model.EventStatusId);
                
            },
            returnParameters: null);
        }

        public List<Event> CreatedBy(int userId)
        {
          
            List<Event> events = null;
            string procName = "[dbo].[Events_Select_ByCreatedBy_V5]";

            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@CreatedBy", userId);
            }, singleRecordMapper: delegate (IDataReader reader, short set)
            {
                Event e = EventMapper(reader);
             
                if (events == null)
                {
                    events = new List<Event>();
                }
                events.Add(e);
            });

            return events;
        }

      

        private static void EventParams(EventAddMultiStep request, int userId, SqlParameterCollection col, DataTable dtEventFiles, DataTable dtVenueFiles)
        {

            col.AddWithValue("@EventTypeId", request.EventTypeId);
            col.AddWithValue("@Name", request.EventName);
            col.AddWithValue("@Summary", request.Summary);
            col.AddWithValue("@ShortDescription", request.ShortDescription);
            col.AddWithValue("@EventStatusId", request.EventStatusId);
            col.AddWithValue("@ImageUrl", request.ImageUrl);
            col.AddWithValue("@ExternalSiteUrl", request.ExternalSiteUrl);
            col.AddWithValue("@IsFree", request.IsFree);
            col.AddWithValue("@EventFiles", dtEventFiles);
            col.AddWithValue("@VenueId", request.VenueId);
            col.AddWithValue("@VenueName", request.Venue.Name);
            col.AddWithValue("@VenueDescription", request.Venue.Description);
            col.AddWithValue("@VenueUrl", request.Venue.Url);
            col.AddWithValue("@VenueFiles", dtVenueFiles);
            col.AddWithValue("@LocationId", request.Venue.LocationId);
            col.AddWithValue("@LocationTypeId", request.Location.LocationTypeId);
            col.AddWithValue("@LocationLineOne", request.Location.LineOne);
            col.AddWithValue("@LocationLineTwo", request.Location.LineTwo);
            col.AddWithValue("@LocationCity", request.Location.City);
            col.AddWithValue("@LocationZip", request.Location.Zip);
            col.AddWithValue("@LocationStateId", request.Location.StateId);
            col.AddWithValue("@LocationLat", request.Location.Latitude);
            col.AddWithValue("@LocationLong", request.Location.Longitude);
            col.AddWithValue("@DateStart", request.DateStart);
            col.AddWithValue("@DateEnd", request.DateEnd);
            col.AddWithValue("@CreatedBy", userId);
            col.AddWithValue("@ModifiedBy", userId);
        }

        private void DataTableMapper (EventAddMultiStep model, out DataTable dtEventFiles, out DataTable dtVenueFiles)
        {
            dtEventFiles = AddMultipleFiles(model.EventFiles);
            dtVenueFiles = AddMultipleFiles(model.Venue.Files);
        }
        private DataTable AddMultipleFiles(List<FileAddRequest> files)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Url", typeof(string));
            dt.Columns.Add("FileTypeId", typeof(int));
            dt.Columns.Add("CreatedBy", typeof(int));

            foreach (var row in files)
            {
                DataRow dr = dt.NewRow();
                dr[0] = row.Url;
                dr[1] = FileUploadService.GetFileTypeIdFromFileName(row.Url);
                dr[2] = row.CreatedBy;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private static Event EventMapper(IDataReader reader)
        {
            int startingIndex = 0;
       
            Event e = new Event();
            e.Id = reader.GetSafeInt32(startingIndex++);
            e.EventTypeId = reader.GetSafeInt32(startingIndex++);
            e.EventType = reader.GetSafeString(startingIndex++);
            e.EventName = reader.GetSafeString(startingIndex++);
            e.Summary = reader.GetSafeString(startingIndex++);
            e.ShortDescription = reader.GetSafeString(startingIndex++);
            e.EventStatusId = reader.GetSafeInt32(startingIndex++);
            e.EventStatus = reader.GetSafeString(startingIndex++);
            e.EventImageUrl = reader.GetSafeString(startingIndex++);
            e.EventLink = reader.GetSafeString(startingIndex++);
            e.IsFree = reader.GetSafeBool(startingIndex++);
            e.DateCreated = reader.GetSafeDateTime(startingIndex++);
            e.DateModified = reader.GetSafeDateTime(startingIndex++);
            e.DateStart = reader.GetSafeDateTime(startingIndex++);
            e.DateEnd = reader.GetSafeDateTime(startingIndex++);
            e.CreatedBy = reader.GetSafeInt32(startingIndex++);
            e.Venue = new Venue();
            e.Venue.Id = reader.GetSafeInt32(startingIndex++);
            e.Venue.Name = reader.GetSafeString(startingIndex++);
            e.Venue.Description = reader.GetSafeString(startingIndex++);
            e.Venue.Url = reader.GetSafeString(startingIndex++);
            e.Venue.Location = new LocationInfo();
            e.Venue.Location.Id = reader.GetSafeInt32(startingIndex++);
            e.Venue.Location.LocationTypeId = reader.GetSafeInt32(startingIndex++);
            e.Venue.Location.LineOne = reader.GetSafeString(startingIndex++);
            e.Venue.Location.LineTwo = reader.GetSafeString(startingIndex++);
            e.Venue.Location.City = reader.GetSafeString(startingIndex++);
            e.Venue.Location.Zip = reader.GetSafeString(startingIndex++);
            e.Venue.Location.Latitude = reader.GetSafeDouble(startingIndex++);
            e.Venue.Location.Longitude = reader.GetSafeDouble(startingIndex++);
            e.Venue.Location.StateId = reader.GetSafeInt32(startingIndex++);
            e.State = new State();
            e.State.Id = reader.GetSafeInt32(startingIndex++);
            e.State.CoutnryId = reader.GetSafeInt32(startingIndex++);
            e.State.StateProvinceCode = reader.GetSafeString(startingIndex++);
            e.State.CountryRegionCode = reader.GetSafeString(startingIndex++);
            e.State.IsOnlyStateProvinceFlag = reader.GetSafeInt32(startingIndex++);
            e.State.Name = reader.GetSafeString(startingIndex++);
            e.State.TerritoryId = reader.GetSafeInt32(startingIndex++);
            e.State.rowguid = reader.GetSafeString(startingIndex++);
            e.State.ModifiedDate = reader.GetSafeDateTime(startingIndex++);
            e.UserProfile = new UserProfile();
            e.UserProfile.Id = reader.GetSafeInt32(startingIndex++);
            e.UserProfile.UserId = reader.GetSafeInt32(startingIndex++);
            e.UserProfile.FirstName = reader.GetSafeString(startingIndex++);
            e.UserProfile.Mi = reader.GetSafeString(startingIndex++);
            e.UserProfile.LastName = reader.GetSafeString(startingIndex++);
            e.UserProfile.AvatarUrl = reader.GetSafeString(startingIndex++);
            e.UserProfile.DateCreated = reader.GetSafeDateTime(startingIndex++);
            e.UserProfile.DateModified = reader.GetSafeDateTime(startingIndex++);
            string files = reader.GetSafeString(startingIndex++);
            if (files != null)
            {
                if (e.Files == null)
                {
                    e.Files = new List<File>();
                }
                e.Files = JsonConvert.DeserializeObject<List<File>>(files);
            }
            return e;
        }
    }     
}
