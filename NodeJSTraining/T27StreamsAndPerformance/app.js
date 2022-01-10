var http = require("http");
var fs = require("fs");

http.createServer(function(req, res) {

//rakennetaan response
    res.writeHead(200, {"Content-type" : "text/html"});
    fs.createReadStream(__dirname+'/index.html').pipe(res);

}).listen(1237, "127.0.0.1");