var http = require("http");
var fs = require("fs");

http.createServer(function(req, res) {

//rakennetaan response
    res.writeHead(200, {"Content-type" : "text/html"});

    var html = fs.readFileSync(__dirname+"/index.html", "utf8");
    var message = "Heippa message";
    html = html.replace("{Message}", message);
    res.end(html);

}).listen(1237, "127.0.0.1");