var fs = require("fs");
var zlib = require("zlib");

var readable = fs.createReadStream(__dirname+"/exampledata.txt", 
{encoding: "utf8", highWaterMark: 16*1024});

var writable = fs.createWriteStream(__dirname+"/exampledatacopy.txt");

var compressed = fs.createWriteStream(__dirname+"/exampledata.txt.gz");

var gzip = zlib.createGzip();

readable.pipe(writable);

// Ketjutettuja funktioita. (Method chaining)
readable.pipe(gzip).pipe(compressed);