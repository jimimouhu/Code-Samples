var util = require('util');

var name = "Matti";
var last = "Meikä";

var greeting = util.format('Moi, %s %s', name, last);

util.log(greeting);