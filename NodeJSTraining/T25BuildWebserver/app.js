var http = require("http");

http.createServer(function(req, res) {

//rakennetaan response
    res.writeHead(200, {"Content-type" : "text/plain"});
    res.end("Heippa hei");

}).listen(1337, "127.0.0.1");