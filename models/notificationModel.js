const mongoose = require("mongoose");

const notificationSchema = new mongoose.Schema({
    date: Date,
    mac: String,
    mallName: String,
    message: String
});

const Notification = mongoose.model('Notification', notificationSchema);

module.exports = Notification;