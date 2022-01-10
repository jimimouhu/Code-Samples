// IIFE = Immediately Invoked Function Expression
// kutsutaan funktiota luonnin yhteydessä ( voi antaa parametreja )
var firstName = "Maija";

(function(lastName){

    var firstName = "Matti";
    console.log(firstName + " " + lastName);

}("Meikäläinen"));