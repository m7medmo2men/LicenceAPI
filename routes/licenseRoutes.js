const express = require("express");
const licenseController = require("./../controllers/licenseController")
const router = express.Router();


/*
router
    .route('/notifyExpirationDate')
    .post(licenseController.notifyExpirationDate)
*/

router
    .route("/sendNotification")
    .post(licenseController.createNotification);

router
    .route('/checkLicense')
    .post(licenseController.checkLicense);

router
    .route('/')
    .get(licenseController.getAllLicense)
    .post(licenseController.addNewLicense)

router
    .route('/:id')
    .get(licenseController.getLicense)
    .patch(licenseController.updateLicense)
    .delete(licenseController.deleteLicense)


module.exports = router;