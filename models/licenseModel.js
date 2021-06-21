const mongoose = require("mongoose");

const licenseSchema = new mongoose.Schema({
    mallName: String,
    active: String,
    mac: String,
    licenseType: String,
    latestProcessingDate: Date,
    startDate: Date,
    endDate: Date,
    licensedUnits: Number,
    licensedUsers: Number,
})

const License = mongoose.model('License', licenseSchema);

module.exports = License;