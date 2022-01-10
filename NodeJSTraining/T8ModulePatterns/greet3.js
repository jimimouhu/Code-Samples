function greetR() {
    this.greeting = "suora funktio";
    this.greet = function() {
        console.log(this.greeting);
    }
}

module.exports = new greetR();