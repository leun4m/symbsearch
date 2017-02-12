const listjs = require('list.js');
const $ = require('jquery'); //would like to remove it
const {clipboard, ipcRenderer} = require('electron');
let symbolarray = require('./data/symbols.json');

let symbols = [];
for (let i=0; i<symbolarray.length; i++) {
  symbols[i] = new symbol();
  symbols[i].name = symbolarray[i][0];
  symbols[i].symbol = symbolarray[i][1];
  symbols[i].cat = symbolarray[i][2];
}

console.log(symbols);

let options = {
  valueNames: [ 'symbol', 'name' ],
  item: '<li><h3 class="symbol"></h3><p class="name"></p></li>'
};
let symbollist = new List('symbollist', options, symbols);

createCatFilter();

function createCatFilter() {
  let cats = getCategories();
  cats.unshift('all');
  let r;
  let form = document.getElementById('cat-filter');
  for (let i=0; i<cats.length; i++) {
    r = document.createElement('option');
    catValid = setValidName(cats[i]);
    r.setAttribute('value', catValid);
    r.setAttribute('tabindex', '-1');
    r.innerHTML = cats[i];
    if (i === 0) {
      r.setAttribute('selected', 'true');
    }
    r.setAttribute('id', 'cat-' + catValid);
    form.addEventListener('change', function() {
      catChange();
    });
    form.appendChild(r);
  }
}

function setValidName(word) {
  return word.replace(/ /g,"_");
}
function setNormalName(word) {
  return word.replace("_",/ /g);
}
function getCategories() {
  let cats = [];
  for (let i=0; i<symbols.length; i++) {
    cats.push(symbols[i].cat);
  }
  cats = uniq_fast(cats);
  return cats;
}

function uniq_fast(a) {
  var seen = {};
  var out = [];
  var len = a.length;
  var j = 0;
  for (let i = 0; i < len; i++) {
    let item = a[i];
    if (seen[item] !== 1) {
      seen[item] = 1;
      out[j++] = item;
    }
  }
  return out;
}
function setFocus() {
  document.getElementById('searchbox').focus();
}

$('input[name="cats"]').on('change', catChange);

function catChange() {
  c = setNormalName(document.getElementById('cat-filter').value);
  symbollist.remove();
  symbollist.add(symbols);
  let cats = getCategories();
  if (c != 'all') {
    for (let i=0; i<cats.length; i++) {
      if (cats[i] != c) {
        symbollist.remove('cat', cats[i])
      }
    }
  }
  setFocus();
}
//http://jsfiddle.net/Vtn5Y/
var tab = $('#cat-filter option');
var catSelected;
let liSelected;
$(window).keydown(function(e){
  let li = $('li');
  if ($('.selected') == null) {
    liSelected = false;
  }
  switch (e.which) {
    case 40:
      if (liSelected){
        liSelected.removeClass('selected');
        next = liSelected.next();
        if (next.length > 0){
          liSelected = next.addClass('selected');
        } else {
          liSelected = li.first().addClass('selected');
        }
      } else {
        console.log("Versuch1")
        liSelected = li.first().addClass('selected');
      }
      $('.selected')[0].scrollIntoView(true);
      break;
    case 38:
      if (liSelected){
        liSelected.removeClass('selected');
        next = liSelected.prev();
        if (next.length > 0){
          liSelected = next.addClass('selected');
        } else {
          liSelected = li.last().addClass('selected');
        }
      } else {
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
      if (catSelected != undefined) {
        next = catSelected.next();
        if (next.length > 0) {
          catSelected = next.prop('selected', true);
        } else {
          catSelected = tab.first().prop('selected', true);
        }
      } else {
        catSelected = tab.first();
        next = catSelected.next();
        catSelected = next;
        catSelected.prop('selected', true);
      }
      catChange();
      liSelected = null;
      break;
    default:
      return;
  }
});

function symbol() {
  this.name;
  this.symbol;
  this.cat;
}

function quit() {
  ipcRenderer.send('asynchronous-message', 'quit');
}
