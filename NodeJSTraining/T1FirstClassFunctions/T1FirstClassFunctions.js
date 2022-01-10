function greet() {
    console.log("Tervetuloa");
}

greet();

function logGreeting(fn) {
    
    fn();

}

logGreeting(greet);


var greetMe = function(){
    console.log("Maanantai");
}

greetMe();

 logGreeting(greetMe);