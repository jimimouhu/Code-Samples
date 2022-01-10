var fs = require("fs");

var readable = fs.createReadStream(__dirname+"/exampledata.txt", 
{encoding: "utf8", highWaterMark: 16*1024});

var writable = fs.createWriteStream(__dirname+"/exampledatacopy.txt");

readable.on("data", function(chunk) {
    console.log(chunk.length);
    writable.write(chunk);
})