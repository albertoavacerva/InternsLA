using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Requests.Location;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Sabio.Services
{
    public class LocationService : ILocationService
    {
        IDataProvider _data = null;

        public LocationService(IDataProvider data)
        {
            _data = data;
        }

        public Location Get(int id)
        {

            string procName = "[dbo].[Locations_SelectById_V2]";
            Location aLocation = new Location();

            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", id);
            }, singleRecordMapper: delegate (IDataReader reader, short set)
            { 
                aLocation = MapLocation(reader);
            }
            );
            return aLocation;

        }

        public Paged<Location> Paginate(int pageIndex, int pageSize)
        {
            Paged<Location> pagedResult = null;
            List<Location> result = null;
            int totalCount = 0;

            string procName = "[dbo].[Locations_SelectAll_V2]";

            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@PageIndex", pageIndex);
                parameterCollection.AddWithValue("@PageSize", pageSize);
            }, singleRecordMapper: delegate (IDataReader reader, short set)
            {
            
                Location aLocation = MapLocation(reader);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(17);
                }
                if (result == null)
                {
                    result = new List<Location>();
                }
                result.Add(aLocation);
            });

            if (result != null)
            {
                pagedResult = new Paged<Location>(result, pageIndex, pageSize, totalCount);
            }
            return pagedResult;
        }

        public Paged<Location> SearchLocation(int pageIndex, int pageSize, string search)
        {
            Paged<Location> pagedResult = null;
            List<Location> result = null;
            int totalCount = 0;

            string procName = "[dbo].[Locations_Search_V2]";


            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@PageIndex", pageIndex);
                parameterCollection.AddWithValue("@PageSize", pageSize);
                parameterCollection.AddWithValue("@Search", search);


            }, singleRecordMapper: delegate (IDataReader reader, short set)
            {
             
                Location aLocation = MapLocation(reader);
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(17);
                }
                if (result == null)
                {
                    result = new List<Location>();
                }
                result.Add(aLocation);
            });
            if (result != null)
            {
                pagedResult = new Paged<Location>(result, pageIndex, pageSize, totalCount);
            }
            return pagedResult;
        }


        public Paged<Location> SearchByRadiusPaginate(LocationSearchRequest model)
        {
            Paged<Location> pagedResult = null;
            List<Location> result = null;
            int totalCount = 0;

            string procName = "[dbo].[Locations_SelectByGeo_V3]";
            

            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@Latitude", model.Latitude);
                parameterCollection.AddWithValue("@Longitude", model.Latitude);
                parameterCollection.AddWithValue("@Radius", model.Radius);
                parameterCollection.AddWithValue("@PageIndex", model.PageIndex);
                parameterCollection.AddWithValue("@PageSize", model.PageSize);
            }, singleRecordMapper: delegate (IDataReader reader, short set)
            {
             
                Location aLocation = MapLocation(reader);
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(17);
                }
                if (result == null)
                {
                    result = new List<Location>();
                }
                result.Add(aLocation);
            });
            if (result != null)
            {
                pagedResult = new Paged<Location>(result, model.PageIndex, model.PageSize, totalCount);
            }
            return pagedResult;
        }

        public int Add(LocationAddRequest model, int userId)
        {
            int id = 0;

            string procName = "[dbo].[Locations_Insert]";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {

                AddCommonParams(model, col);
                col.AddWithValue("@ModifiedBy", userId);
                col.AddWithValue("@CreatedBy", userId);


                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;

                col.Add(idOut);
            }, returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                object oId = returnCollection["@Id"].Value;
                int.TryParse(oId.ToString(), out id);
            });
            return id;
        }

        public void Update(LocationUpdateRequest model, int userId)
        {
            string procName = "[dbo].[Locations_Update]";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(model, col);
                col.AddWithValue("@ModifiedBy", userId);
                col.AddWithValue("@Id", model.Id);
            }, returnParameters: null);
        }

        public void Delete(int Id)
        {
            string procName = "[dbo].[Locations_Delete_ById]";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", Id);
            }, returnParameters: null);
        }
        private static Location MapLocation(IDataReader reader)
        {
            int index = 0;
            Location aLocation = new Location();
            State state = new State();

            aLocation.Id = reader.GetSafeInt32(index++);    
            aLocation.LocationTypeName = reader.GetSafeString(index++);
            aLocation.LocationTypeId = reader.GetSafeInt32(index++);
            aLocation.LineOne = reader.GetSafeString(index++);
            aLocation.LineTwo = reader.GetSafeString(index++);
            aLocation.City = reader.GetSafeString(index++);
            aLocation.Zip = reader.GetSafeString(index++);
            state.Id = reader.GetSafeInt32(index++);
            state.Name = reader.GetSafeString(index++);
            state.StateProvinceCode = reader.GetSafeString(index++);
            state.CountryRegionCode = reader.GetSafeString(index++);
            aLocation.Latitude = reader.GetSafeDouble(index++);
            aLocation.Longitude = reader.GetSafeDouble(index++);
            aLocation.DateCreated = reader.GetSafeDateTime(index++);
            aLocation.DateModified = reader.GetSafeDateTime(index++);
            aLocation.FirstName = reader.GetSafeString(index++);
            aLocation.LastName = reader.GetSafeString(index++);

            aLocation.State = state;
            return aLocation;
        }

        private static void AddCommonParams(LocationAddRequest model, SqlParameterCollection col)
        {
            col.AddWithValue("@LocationTypeId", model.LocationTypeId);
            col.AddWithValue("@LineOne", model.LineOne);
            col.AddWithValue("@LineTwo", model.LineTwo);
            col.AddWithValue("@City", model.City);
            col.AddWithValue("@Zip", model.Zip);
            col.AddWithValue("@StateId", model.StateId);
            col.AddWithValue("@Latitude", model.Latitude);
            col.AddWithValue("@Longitude", model.Longitude);
        }

        public Paged<Location> Search(int pageIndex, int pageSize, string search)
        {
            Paged<Location> pagedResult = null;
            List<Location> result = null;
            int totalCount = 0;

            string procName = "[dbo].[Locations_Search_V2]";


            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@PageIndex", pageIndex);
                parameterCollection.AddWithValue("@PageSize", pageSize);
                parameterCollection.AddWithValue("@Search",search);
                
               
            }, singleRecordMapper: delegate (IDataReader reader, short set)
            {
                Location aLocation = MapLocation(reader);
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(17);
                }
                if (result == null)
                {
                    result = new List<Location>();
                }
                result.Add(aLocation);
            });
            if (result != null)
            {
                pagedResult = new Paged<Location>(result,pageIndex, pageSize, totalCount);
            }
            return pagedResult;
        }
    }
}

