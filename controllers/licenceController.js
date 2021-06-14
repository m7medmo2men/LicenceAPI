const Licence = require("./../models/licenceModel.js")

// [DONE]
exports.getAllLicence = async (req, res, next) => {

  try {
    const licences = await Licence.find();
    res.status(200).json({
      status: "success",
      data: licences,
    });
  } catch(err) {
    res.status(404).json({
      status: "fail",
      message: err,
    });
  }
};

/// [DONE]
exports.addNewLicence = async (req, res, next) => {
  
  try {
    const newLicence = await Licence.create(req.body);
    res.status(200).json({
      status: "success",
      message: "licence added successfully",
      data: newLicence,
    });
  } catch (err) {
    res.status(404).json({
      status: "fail",
      message: err,
    });
  }
};

/// [DONE]
exports.getLicence = async (req, res, next) => {
  console.log("HWA FEE EHH")
  try {
    const licence = await Licence.findById(req.params.id);
    res.status(200).json({
      status: "success",
      data: licence,
    });
  } catch (err) {
    res.status(404).json({
      status: "fail",
      message: err
    });
  }

};

/// [Done]
exports.updateLicence = async (req, res, next) => {

  const licence = await Licence.findByIdAndUpdate(req.params.id, req.body, {
    new: true
  })
  try {
    res.status(200).json({
      status: "success",
      data: licence
    }) 
  } catch(err) {
    res.status(404).json({
      status: "fail",
      message: err
    });
  }
  
  
  /*const id = +req.params.id;
  for (let i = 0; i < licences.length; i++) {
    if (licences[i].id === id) {
      const updatedLicence = Object.assign({ id: id }, req.body);
      licences[i] = updatedLicence;
      fs.writeFileSync(
        `${__dirname}/../data/licences.json`,
        JSON.stringify(licences)
      );
      return res.status(200).json({
        status: "success",
        message: "licence updated successfully",
        data: licences[i],
      });
    }
  }

  return res.status(404).json({
    status: "fail",
    message: "Licence not found",
  });*/
};

/// [Done]
exports.deleteLicence = async (req, res, next) => {
  console.log(req.params.id);
  try {
    await Licence.findByIdAndDelete(req.params.id);
    // JSON DOES NOT SHOW
    res.status(204).json({
      status: "success licence deleted successfully",
      message: "Licence Deleted Successfully",
    })
  } catch (err) {
    res.status(404).json({
      status: "failed",
      message: "Deleting Licence Failed",
      error: err
    })
  }
};

exports.checkLicence = async (req, res, next) => {
  
  let licence;
  try {
    licence = await Licence.findOne({mac: req.body.mac});
  } catch (err) {
    console.log(err);
  }

  if (licence == null) {
    return res.status(404).json({
      status: "Fail",
      message: "No Licence Found",
      data: licence
    });
  }

  let dateToday = new Date().toISOString();
  updateLatestProcessingDateOfLicense(req.body.mac, dateToday);
  console.log(dateToday);

  try {
    licence = await Licence.find({mac: req.body.mac});
  } catch (err) {
    // Cannot Be Tested, I can't drop the database connection manually
    return res.status(404).json({
      status: "Fail",
      message: `Something Went Wrong, \n ${err.message}}`
    });
  }

  res.status(200).json({
    status: "success",
    message: licence,
  });
};

exports.createNotification = (req, res, next) => {
  console.log(req.body);
  const newNotification = Object.assign(
    { id: notifications.length + 1 },
    { date: new Date().toISOString() },
    { mac: req.body.mac },
    { mallName: req.body.mallName },
    { message: req.body.message }
  );

  notifications.push(newNotification);
  fs.writeFileSync(
    `${__dirname}/../data/notifications.json`,
    JSON.stringify(notifications)
  );

  return res.status(200).json({
    status: "success",
    message: "notification added successfully",
  });
};


const updateLatestProcessingDateOfLicense = async (mac, latestProcessingDate) => {
  try {
    await Licence.updateOne({mac: mac}, {$set: {latestProcessingDate: latestProcessingDate}});
  } catch (err) {
    console.log(err);
    console.log("Failed To Update The Date");
  }
};