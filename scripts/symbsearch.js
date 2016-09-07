//PARSE JSON
//var symbols = ...
//if txtchange
let symbols = require('./data/symbols.json').symbols;

window.onload = init;

function init() {
  let main = document.querySelector('main');
  console.log(symbols.length)
  for (let i=0; i<symbols.length; i++) {
    let d = document.createElement('div');
    d.id = "symb-" + symbols[i].name;
    let h = document.createElement('h3');
    h.innerHTML = symbols[i].symbol;
    let p = document.createElement('p');
    p.innerHTML = symbols[i].name;
    d.appendChild(h);
    d.appendChild(p);
    main.appendChild(d);
    console.log(symbols[i].symbol);
  }
}

/*
function textChange() {
  var stxt = document.querySelector('#searchbox').value();

  for (i=0; i>symbols.length(); i++) {
    //instead of "!=" a "hasn't in it" search
    var x = "\ "+symbols[i].name
    if(symbols[i].name != stxt) {
      document.querySelector("#symb-"+symbols[i].name) //remove from DOM
    }
  }
}*/

//IF ENTER:

//copy symbols[0].symbol to clipboard
//minimize
