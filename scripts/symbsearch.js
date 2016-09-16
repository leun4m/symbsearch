const listjs = require('list.js');
const $ = require('jquery'); //would like to remove it
const {clipboard, ipcRenderer} = require('electron');
let symbols = require('./data/symbols.json').symbols;

let options = {
    valueNames: [ 'symbol', 'name' ],
    item: '<li><h3 class="symbol"></h3><p class="name"></p></li>'
};
let symbollist = new List('symbollist', options, symbols);

createCatFilter()

function createCatFilter() {
  let cats = []
  //Task: Get every Category
  /*for (let i=0; i<symbols.length; i++) {
    cats[i].append(symbols[i].cat);
  }*/
  //cats = getDoubles(cats)
  cats = ["greek", "arrows"]
  let r;
  let area = document.getElementById("cat-filter")
  for (let i=0; i<cats.length; i++) {
    r = document.createElement("button");
    r.innerHTML = cats[i];
    area.appendChild(r);
  }
}


function getDoubles(a) {
  for (let i=0; i<a.length-1; i++) {
    if (a[i] === a[i+1]) {
      a.slice(i,i+1)
    }
  }
}
$('button').addEventListener("clicked", () => {
  //filter
})
//http://jsfiddle.net/Vtn5Y/
var li = $('li');
var liSelected;
$(window).keydown(function(e){
  if($('.selected') == null) {
    liSelected = false;
  }
    if(e.which === 40){
        if(liSelected){
            liSelected.removeClass('selected');
            next = liSelected.next();
            if(next.length > 0){
                liSelected = next.addClass('selected');
            }else{
                liSelected = li.eq(0).addClass('selected');
            }
        }else{
            liSelected = li.eq(0).addClass('selected');
        }
    }else if(e.which === 38){
        if(liSelected){
            liSelected.removeClass('selected');
            next = liSelected.prev();
            if(next.length > 0){
                liSelected = next.addClass('selected');
            }else{
                liSelected = li.last().addClass('selected');
            }
        }else{
            liSelected = li.last().addClass('selected');
        }
    }
    else if (e.which === 13) {
      var d = $('.selected h3').text()
      clipboard.writeText(d)
      console.log(clipboard.readText())
      //MESSAGE
      ipcRenderer.send('asynchronous-message', 'hide');
    }
});

function quit() {
  ipcRenderer.send('asynchronous-message', 'quit');
}
