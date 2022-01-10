var http = require("http");
var fs = require("fs");

http.createServer(function(req, res) {

//rakennetaan response
    res.writeHead(200, {"Content-type" : "text/json"});
    
    var person = {
        firstName: "Jimi",
        lastName: "Mouhu"
    };

    res.end(JSON.stringify(person));

}).listen(1337, "127.0.0.1");