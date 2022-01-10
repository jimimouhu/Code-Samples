function Emitter(){
    this.events = {};
}

// Kaksi funktiota; ON-funktio ja EMIT-funktio.


// ON
Emitter.prototype.on = function(type, listener){
    // Type = tapahtuma, listener = funktio

    this.events[type] = this.events[type] || [];
    this.events[type].push(listener);
}

// EMIT
Emitter.prototype.emit = function(type){
    if (this.events[type]) {
        this.events[type].forEach(function(listener){
            listener();
        });
    }
}

module.exports = Emitter;