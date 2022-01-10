function greetR() {
    this.greeting = "greet4";
    this.greet = function() {
        console.log(this.greeting);
    }
}
module.exports = greetR;