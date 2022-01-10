function greet(callback) {
    console.log("heippa");
    // Tämä funktio lähetää dataa callbackille lopputuloksena

    var data = {
        name: "Matti Makkonen"
    }

    callback(data);

}

greet(function(data){
    console.log("Callback on ajettu");
    console.log(data);
});

greet(function(data){
    console.log("Callback on ajettu 2");
    console.log(data.name);
});

