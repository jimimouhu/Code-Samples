var fs = require("fs");

var greet = fs.readFileSync(__dirname+"/greet.txt", "utf8");
console.log(greet);

var greet2 = fs.readFile(__dirname+"/greet.txt", "utf8", 

function(err, data) {
    console.log(data); // tämä on tiedosto joka on luettu
});

console.log("Tämä suoriutuu ennen greet2, koska greet2 on asynkroninen");
console.log("Tämä suoriutuu ennen greet2, koska greet2 on asynkroninen");
console.log("Tämä suoriutuu ennen greet2, koska greet2 on asynkroninen");
console.log("Tämä suoriutuu ennen greet2, koska greet2 on asynkroninen");
console.log("Tämä suoriutuu ennen greet2, koska greet2 on asynkroninen");
console.log("Tämä suoriutuu ennen greet2, koska greet2 on asynkroninen");
