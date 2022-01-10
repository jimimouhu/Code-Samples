
'use strict'
var EventEmitter = require('events');
module.exports = class GreetR extends EventEmitter {
    constructor() 
    {
        super(); //EventEmitter.call(this);
        this.greeting = "Mahtavaa tiistaita";
    }
    greet(data) {
        console.log(this.greeting);
        this.emit('greet', data);
    }
}