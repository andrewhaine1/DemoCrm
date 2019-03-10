// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

/*------------ Side Menu Tree -------------*/
var toggler = document.getElementsByClassName("caret");
var i;

for (i = 0; i < toggler.length; i++) {
    toggler[i].addEventListener("click", function () {
        this.parentElement.querySelector(".company-side-menu-nested").classList.toggle("company-side-menu-active");

        //console.log(this.nextSibling.nextSibling.offsetHeight);

        if (this.parentElement.nextSibling.nextSibling != null) {
            if (this.parentElement.nextSibling.nextSibling.style.marginTop != '150px') {
                console.log(this.parentElement.nextSibling.nextSibling.style.marginTop = '150px');
            } else {
                console.log(this.parentElement.nextSibling.nextSibling.style.marginTop = '0');
            }
        }

        this.classList.toggle("caret-down");
    });
}