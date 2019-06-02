// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

class FormUtil {
    constructor(config) {
        //super();
        this.scene = config.scene;
        //get the game height and width
        this.gameWidth = this.scene.game.config.width;
        this.gameHeight = this.scene.game.config.height;
        this.alignGrid = new AlignGrid({
            scene: this.scene,
            rows: config.rows,
            cols: config.cols
        });
    }
    showNumbers() {
        this.alignGrid.showNumbers();
    }
    scaleToGameW(elName, per) {
        var el = document.getElementById(elName);
        var w = this.gameWidth * per;
        el.style.width = w + "px";
    }
    scaleToGameH(elName, per) {
        var el = document.getElementById(elName);
        var h = this.gameHeight * per;
        el.style.height = h + "px";
    }
    placeElementAt(index, elName, centerX = true, centerY = false) {
        //get the position from the grid
        var pos = this.alignGrid.getPosByIndex(index);
        //extract to local vars
        var x = pos.x;
        var y = pos.y;
        //get the element
        var el = document.getElementById(elName);
        //set the position to absolute
        el.style.position = "absolute";
        //get the width of the element
        var w = el.style.width;
        //convert to a number
        w = this.toNum(w);
        //
        //
        //center horizontal in square if needed
        if (centerX == true) {
            x -= w / 2;
        }
        //
        //get the height
        //        
        var h = el.style.height;
        //convert to a number
        h = this.toNum(h);
        //
        //center verticaly in square if needed
        //
        if (centerY == true) {
            y -= h / 2;
        }
        //set the positions
        el.style.top = y + "px";
        el.style.left = x + "px";
    }
    //changes 100px to 100
    toNum(s) {
        s = s.replace("px", "");
        s = parseInt(s);
        return s;
    }
    //add a change callback
    addChangeCallback(elName, fun, scope = null) {
        var el = document.getElementById(elName);
        if (scope == null) {
            el.onchange = fun;
        } else {
            el.onchange = fun.bind(scope);
        }
    }
    getTextAreaValue(elName) {
        var el = document.getElementById(elName);
        return el.value;
    }
    getTextValue(elName) {
        var el = document.getElementById(elName);
        return el.innerText;
    }
}

var config = {
    type: Phaser.AUTO,
    width: 800,
    height: 600,
    backgroundColor: '#000000',
    parent: 'phaser-example',
    scene: {
        preload: preload,
        create: create,
        update: update
    }
};

var t = 0;
var image0, image1;
var game = new Phaser.Game(config);
var fu = new FormUtil();

function preload() {
  
}

function create() {
   // this.formUtil.addOption("optlist", "first", {
   //     id: 1
    // });
    this.fu.scaleToGameW("optlist", 0.8);
    this.fu.placeElementAt(29, "optlist");
}

function update() {
   
}
