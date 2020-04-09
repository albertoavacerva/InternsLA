import * as Yup from "yup";

const locationValidationSchema = Yup.object().shape({

    locationTypeId: Yup.number(),
    lineOne: Yup.string()
        .min(1, "The Address is Invalid")
        .required("Required"),
    lineTwo: Yup.string(),
    city: Yup.string()
        .min(2, "The City is Invalid")
        .required("Required"),
    zip: Yup.string()
        .min(5, "The Zip Code is Invalid - Too Short")
        .max(10, "The Zip Code is Invalid- Too Long")
        .required("Required"),
    stateId: Yup.number(),
});

export { locationValidationSchema }

