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
  cats.unshift('all');
  let r, l;
  let form = document.getElementById('cat-filter');
  for (let i=0; i<cats.length; i++) {
    r = document.createElement('input');
    l = document.createElement('label');
    r.setAttribute('type', 'radio');
    r.setAttribute('name', 'cats');
    r.setAttribute('value', cats[i]);
    r.setAttribute('tabindex', '-1');
    if (i === 0) {
      r.checked = true;
    }
    r.setAttribute('id', 'cat-' + cats[i]);
    l.setAttribute('for','cat-' + cats[i]);
    l.innerHTML = cats[i];
    form.appendChild(r);
    form.appendChild(l);
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


$('input[name="cats"]').on('change', catChange);

function catChange() {
  let c = $('input[name="cats"]:checked').val();
  symbollist.remove()
  symbollist.add(symbols)
  let cats = getCategories()
  if (c != 'all') {
    for (let i=0; i<cats.length; i++) {
      if (cats[i] != c) {
        symbollist.remove('cat', cats[i])
      }
    }
  }
}
//http://jsfiddle.net/Vtn5Y/
var tab = $('#cat-filter input');
var catSelected;
$(window).keydown(function(e){
    let li = $('li');
    let liSelected;
    if($('.selected') == null) {
      liSelected = false;
    }
    switch (e.which) {
      case 40:
        if(liSelected){
            liSelected.removeClass('selected');
            next = liSelected.next();
            if(next.length > 0){
                liSelected = next.addClass('selected');
            }else{
                liSelected = li.first().addClass('selected');
            }
        }else{
          console.log("Versuch1")
            liSelected = li.first().addClass('selected');
        }
        $('.selected')[0].scrollIntoView(true);
        break;
      case 38:
        if(liSelected){
            liSelected.removeClass('selected');
            next = liSelected.prev();
            if(next.length > 0){
                liSelected = next.addClass('selected');
            }else{
                liSelected = li.last().addClass('selected');
            }
        }else{
          console.log("Versuch2")
            liSelected = li.last().addClass('selected');
        }
        $('.selected')[0].scrollIntoView(true);
        break;
      case 13: //enter
        var d = $('.selected h3').text()
        clipboard.writeText(d)
        console.log(clipboard.readText())
        $('#searchbox').select();
        //MESSAGE
        ipcRenderer.send('asynchronous-message', 'hide');
        break;
      case 18: //alt
        if(catSelected != undefined){
            next = catSelected.next();
            if(next.length > 0){
                catSelected = next.prop('checked', true);
            }else{
                catSelected = tab.first().prop('checked', true);
            }
        }else{
            catSelected = tab.first().prop('checked', true);
        }
        catChange();
        $('input:checked')[0].scrollIntoViewIfNeeded(true);
        liSelected = null;
        break;
      default:
        return;
    }
});

function quit() {
  ipcRenderer.send('asynchronous-message', 'quit');
}
