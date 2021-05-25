const cors = require('cors');
const express = require("express");
const licenceController = require("./../controllers/licenceController")
const router = express.Router();


/*
router
    .route('/notifyExpirationDate')
    .post(licenceController.notifyExpirationDate)
*/

router
    .route("/sendNotification")
    .post(licenseController.createNotification);
router
    .route('/disableLicence')
    .post(licenceController.disableLicence);

router
    .route('/checkLicence')
    .post(licenceController.checkLicence);

router
    .route('/')
    .get(licenceController.getAllLicence)
    .post(licenceController.addNewLicence)

router
    .route('/:id')
    .get(licenceController.getLicence)
    .patch(licenceController.updateLicence)
    .delete(licenceController.deleteLicence)


module.exports = router;