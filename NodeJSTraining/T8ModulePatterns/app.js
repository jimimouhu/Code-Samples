// greet1
var greet = require('./greet');
greet();

// greet2
var greet2 = require('./greet2').greet;
greet2();

// greet3
var greet3 = require('./greet3');

greet3.greet();

greet3.greeting = "muutettu tekstiä";

var greet3b = require('./greet3');
greet3b.greet();

// greet4
var greet4 = require('./greet4');
var grt = new greet4();
grt.greet();
// uusi objekti, uudessa muistipaikassa
var grt2 = new greet4();
grt2.greeting = "grt2.greet4";
grt2.greet();

//greet5
var greet5 = require('./greet5').greetN;
var greet5b = require('./greet5').heippaN;
greet5();
greet5b();