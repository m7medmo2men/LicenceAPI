var fetch = require("node-fetch")
var express = require('express')
var cors = require('cors')
var app = express()

/*app.get('/products/:id', function (req, res, next) {
  res.json({msg: 'This is CORS-enabled for an allowed domain.'})
})*/


/*const requestIp = require('request-ip');
app.use(requestIp.mw())
app.use(function(req, res) {
    const ip = req.clientIp;
    res.end(ip);
});*/

app.listen(4000, function () {
  console.log('web server listening on port 4000')
})


fetch("https://localhost:8443/licences/1")
    .then(response => response.json())
    .then(data => console.log(data))
    .catch(err => console.log(err));


