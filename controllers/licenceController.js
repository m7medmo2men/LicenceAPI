const express = require("express");
const fs = require("fs");

let licences = JSON.parse(
    fs.readFileSync(`${__dirname}/../data/licences.json`)
);

/// [Done]
exports.getAllLicence = (req, res, next) => {
    //res.header("Access-Control-Allow-Origin", "*");
    res.status(200).json({
        status: "success",
        data: licences
    })
}



exports.addNewLicence = (req, res, next) => {
    const nextID = licences.length + 1;
    const newLicence = Object.assign({id: nextID}, req.body);
    licences.push(newLicence);
    fs.writeFileSync(`${__dirname}/../data/licences.json`, JSON.stringify(licences));
    return res.status(200).json({
        status: "success",
        message: "licence added successfully",
        data: newLicence
    })
}


exports.getLicence = (req, res, next) => {
    
    const id = +req.params.id;
    if (id > licences.length) {
        res.status(404).json({
            status: "fail",
            message: "Licence not found",
        })
    }

    const licence = licences.find(el => el.id === id);
    return res.status(200).json({
        status: "Success",
        data: licence
    })

    
}


/// [Done]
exports.updateLicence = (req, res, next) => {
    
    const id = +req.params.id;
    for (let i = 0; i < licences.length; i++) {
        if (licences[i].id === id) {
            const updatedLicence = Object.assign({id: id}, req.body);
            licences[i] = updatedLicence;
            fs.writeFileSync(`${__dirname}/../data/licences.json`, JSON.stringify(licences));
            return res.status(200).json({
                status: "success",
                message: "licence updated successfully",
                data: licences[i]
            })
        }
    }

    return res.status(404).json({
        status: "fail",
        message: "Licence not found",
    });
}


/// [Done]
exports.deleteLicence = (req, res, next) => {
    
    const size = licences.length;
    licences = licences.filter(el => el.id !== +req.params.id);
    if (licences.length === size) { // Nothing was deleted
        return res.status(404).json({
            status: "failed",
            message: "No Licence Deleted"
        })
    }

    fs.writeFileSync(`${__dirname}/../data/licences.json`, JSON.stringify(licences));
 
    return res.status(200).json({
        status: "success licence deleted successfully",
        message: "Licence Deleted Successfully"
    })
}

exports.checkLicence = (req, res, next) => {
    console.log(req.body.mac)
    const licence = licences.find(el => el.mac === req.body.mac);
    if (licence === undefined) {
        res.status(401).json({
            status: "fail",
            message: "This Machine has no access for portal"
        })
    } else {
        res.status(200).json({
            status: "success",
            message: licence,
        })
    }
}

exports.disableLicence = (req, res, next) => {
    const licence = licences.find(el => el.mac === req.body.mac);
    if (licence !== undefined) {
        for (let i = 0; i < licences.length; i++) {
            if (licences[i].mac === req.body.mac) {
                licences[i].Active = "N";
                fs.writeFileSync(`${__dirname}/../data/licences.json`, JSON.stringify(licences));
                return res.status(200).json({
                    status: "success",
                    message: "licence disables successfully",
                    data: licences[i]
                })
            }
        }
    }
}