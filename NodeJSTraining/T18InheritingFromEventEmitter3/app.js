// ladataan noden kaksi core moduulia
// Events ja Utils
// Utils tarjoaa mahdollisuuden perintään
'use strict';

var GreetR = require('./greet');


 var greeter1 = new GreetR();

 greeter1.on('greet', function() {
    console.log("joku tervehdys");
 });

 greeter1.greet();