const License = require("./../models/licenseModel.js");
const Notification = require("./../models/notificationModel.js");

// [DONE]
exports.getAllLicense = async (req, res, next) => {

  try {
    const licenses = await License.find();
    res.status(200).json({
      status: "success",
      data: licenses,
    });
  } catch(err) {
    res.status(404).json({
      status: "fail",
      message: err,
    });
  }
};

/// [DONE]
exports.addNewLicense = async (req, res, next) => {
  
  try {
    const newLicense = await License.create(req.body);
    res.status(200).json({
      status: "success",
      message: "license added successfully",
      data: newLicense,
    });
  } catch (err) {
    res.status(404).json({
      status: "fail",
      message: err,
    });
  }
};

/// [DONE]
exports.getLicense = async (req, res, next) => {
  try {
    const license = await License.findById(req.params.id);
    res.status(200).json({
      status: "success",
      data: license,
    });
  } catch (err) {
    res.status(404).json({
      status: "fail",
      message: err
    });
  }

};

/// [Done]
exports.updateLicense = async (req, res, next) => {

  const license = await License.findByIdAndUpdate(req.params.id, req.body, {
    new: true
  })
  try {
    res.status(200).json({
      status: "success",
      data: license
    }) 
  } catch(err) {
    res.status(404).json({
      status: "fail",
      message: err
    });
  }
};

/// [Done]
exports.deleteLicense = async (req, res, next) => {
  console.log(req.params.id);
  try {
    await License.findByIdAndDelete(req.params.id);
    // JSON DOES NOT SHOW
    res.status(204).json({
      status: "success license deleted successfully",
      message: "License Deleted Successfully",
    })
  } catch (err) {
    res.status(404).json({
      status: "failed",
      message: "Deleting License Failed",
      error: err
    })
  }
};

exports.checkLicense = async (req, res, next) => {
  
  let license;
  try {
    license = await License.findOne({mac: req.body.mac});
  } catch (err) {
    console.log(err);
  }

  if (license == null) {
    return res.status(404).json({
      status: "Fail",
      message: "No License Found",
      data: license
    });
  }

  let dateToday = new Date().toISOString();
  updateLatestProcessingDateOfLicense(req.body.mac, dateToday);
  console.log(dateToday);

  try {
    license = await License.findOne({mac: req.body.mac});
  } catch (err) {
    // Cannot Be Tested, I can't drop the database connection manually
    return res.status(404).json({
      status: "Fail",
      message: `Something Went Wrong, \n ${err.message}}`
    });
  }

  res.status(200).json({
    status: "success",
    message: license,
  });
};

exports.createNotification = async (req, res, next) => {

  try {
    const newNotification = Object.assign(
      { date: new Date().toISOString() },
      { mac: req.body.mac },
      { mallName: req.body.mallName },
      { message: req.body.message }
    );
    await Notification.create(newNotification);
    res.status(200).json({
      status: "success",
      message: "notification added successfully",
      data: newNotification,
    });

  } catch (err) {
    return res.status(404).json({
      status: "fail",
      message: "failed to add notification",
      error: err
    });
  }
};


const updateLatestProcessingDateOfLicense = async (mac, latestProcessingDate) => {
  try {
    await License.updateOne({mac: mac}, {$set: {latestProcessingDate: latestProcessingDate}});
  } catch (err) {
    console.log(err);
    console.log("Failed To Update The Date");
  }
};