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
  let cats = getCategories();
  let r;
  let area = document.getElementById("cat-filter")
  for (let i=0; i<cats.length; i++) {
    r = document.createElement("button");
    r.id = "cat-" + cats[i];
    r.innerHTML = cats[i];
    area.appendChild(r);
  }
}

function getCategories() {
  let cats = [];
  for (let i=0; i<symbols.length; i++) {
    cats.push(symbols[i].cat);
  }
  cats = uniq_fast(cats)
  return cats;
}

function uniq_fast(a) {
    var seen = {};
    var out = [];
    var len = a.length;
    var j = 0;
    for(let i = 0; i < len; i++) {
         let item = a[i];
         if(seen[item] !== 1) {
               seen[item] = 1;
               out[j++] = item;
         }
    }
    return out;
}


$('button').click((e) => {
  c = e.target.id.replace('cat-','')
  symbollist.remove()
  symbollist.add(symbols)
  let cats = getCategories()
  for (let i=0; i<cats.length; i++) {
    if (cats[i] != c) {
      symbollist.remove('cat', cats[i])
    }
  }
});


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
