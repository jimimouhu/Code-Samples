var Emitter = require('./emitter');

var emt = new Emitter();

// Rakennetaan EMITTERI
// Kun tapahtuu asia "greet", tämä funktio käynnistyy.

emt.on('greet', function(){
    console.log("emitter tervehtii");
});

emt.on('lol', function(){
    console.log("lol!");
});

console.log("emitterin testitervehdys");
emt.emit('greet');
emt.emit('lol');