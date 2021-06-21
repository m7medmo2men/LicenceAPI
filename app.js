const cors = require('cors');
const https = require('https');
const path = require('path');
const dotenv = require('dotenv');
const fs = require('fs');
const mongoose = require("mongoose");
const express = require('express');
const licenseRouter = require("./routes/licenseRoutes");

const port = 3000;
const app = express();

dotenv.config({path: './config.env'});

const DB = process.env.DATABASE.replace('<PASSWORD>', process.env.DATABASE_PASSWORD);

app.use(express.json());
 
app.use("/licenses", licenseRouter);

app.listen(port, () => { 
    console.log(`App running on port ${port}...`);
})


mongoose.connect(DB, {
    useNewUrlParser: true,
    useCreateIndex: true,
    useFindAndModify: false
}).then(() => {
    console.log("DB Connection Successfully");
}).catch((err) => {
    console.log(err)
    console.log("Failed To Connect To DB connection");
})

/*
const sslServer = https.createServer({
    key: fs.readFileSync(`${__dirname}/cert/key.pem`),
    cert: fs.readFileSync(`${__dirname}/cert/cert.pem`), 
    requestCert: false, 
    rejectUnauthorized: false
}, app)



sslServer.listen(port, () => {
    console.log(`App running on port ${port}...`);
});
*/

/*
//const allowedOrigins = ['http://127.0.0.1:4000', 'http://localhost:8020', 'http://127.0.0.1:9000', 'http://localhost:9000'];
const allowedOrigins = ['http://10.20.62.40:3000'];

app.use((req, res, next) => {
    const recievedOrigin = req.protocol + "://" + req.headers.host;
    console.log(recievedOrigin);
    const origin = req.headers.origin;
    console.log("ORRRIIGIIN" , origin); // --> undefined

    if (allowedOrigins.includes(recievedOrigin)) {
     //if (allowedIPs.includes(host)) {
        res.setHeader('Access-Control-Allow-Origin', recievedOrigin);
        res.header('Access-Control-Allow-Methods', 'GET, OPTIONS');
        res.header('Access-Control-Allow-Headers', 'Content-Type, Authorization');
        res.header('Access-Control-Allow-Credentials', true);
        next();
    } else {
        //res.end();   
        res.status(401).json({
            status: "fail",
            message: "Ma7dsh yegy gamb el api beta3yyyyy ",
        })
    } 
});
*/

/*
const allowlist = ['http://something.com', 'http://example.com'];

const corsOptionsDelegate = (req, callback) => {
    let corsOptions;

    let isDomainAllowed = allowlist.indexOf(req.header('Origin')) !== -1;
    console.log(isDomainAllowed)
    if (isDomainAllowed) {
        // Enable CORS for this request
        corsOptions = { origin: true }
    } else {
        // Disable CORS for this request
        corsOptions = { origin: false }
    }
    callback(null, corsOptions)
}

app.use(cors(corsOptionsDelegate));
*/

/*
let blackList = ['10.20.62.74', '127.0.0.1', '10.20.62.77']
app.use((req, res, next) => {
    let ip = req.ip;
    console.log(ip)
    if (ip.substr(0, 7) === "::ffff:") {
        ip = ip.substr(7)
    }
    console.log(ip) 
    if(blackList.indexOf(ip) > -1) {
        res.status(401).json({
            status: "fail",
            message: "Ma7dsh Hyegy Henaaaa, Ma7dsh hygy gamb el api beta3yyyyy ",
        })
    } else {
        next();
    }
    
});*/


// app.use("/licenses", licenseRouter);







// app.set('trust proxy', true);

/*
app.use((req, res, next) => {
    //res.header("Access-Control-Allow-Origin", "http://your-app.com")
    //next();
    console.log(req.connection.remoteAddress);
    console.log(req.ip);
    console.log(req.headers.origin);
//    console.log(req.header('X-Real-IP'));
    console.log(req.socket.remoteAddress);
})*/
