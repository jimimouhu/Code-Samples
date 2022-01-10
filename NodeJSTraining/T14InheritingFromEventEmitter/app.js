// ladataan noden kaksi core moduulia
// Events ja Utils
// Utils tarjoaa mahdollisuuden perintään

var EventEmitter = require('events');
var util = require ('util');

// Konstruktori meidän omalle objektille
// Objektilla on ominaisuus greeting, ei oikeastaan muuta.


function GreetR(){
    this.greeting = "Mahtavaa tiistaita";
}

// GreetR objektin prototyypille kerrotaan, että sen protoyyppi on EventEmitterin
// prototyyppi. Näin ollen, GreetR saa samat ominaisuudet kuin EventEmitter
// Eli GreetR on meidän luoma objekti, mutta samalla myös EventEmitter.

 util.inherits(GreetR, EventEmitter);

 GreetR.prototype.greet = function(){
     console.log(this.greeting);
     this.emit('greet');
 }

 var greeter1 = new GreetR();

 greeter1.on('greet', function() {
    console.log("joku tervehdys");
 });

 greeter1.greet();
