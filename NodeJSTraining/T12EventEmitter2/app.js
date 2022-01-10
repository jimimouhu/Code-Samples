var Emitter = require('events');

var eventCfg = require('./config').events;

var emt = new Emitter();

emt.on(eventCfg.GREET, function(){
    console.log("emitter2 tervehtii");
});
emt.on('greet2', function(){
    console.log("emitter2 tervehtii toisen kerran");
});
emt.on('greet2', function(){
    console.log("emitter2 tervehtii kolmannen kerran!!");
});

console.log("Emitoidaan");
emt.emit("greet");
emt.emit("greet2");


