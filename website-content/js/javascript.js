// Nav bar

function clickSmallNavMenu() {
  var smallNavBar = document.getElementById("small-nav-bar");
    
  if(smallNavBar.className.indexOf("w3-show") != -1) {
      smallNavBar.className = smallNavBar.className.replace(" w3-show", "");
  }
  else {
      smallNavBar.className += " w3-show";
  }
}

// Gallery/slide show

var slideIndex = 1;
showDivs(slideIndex);

function plusDivs(n) {
  showDivs(slideIndex += n);
}

function showDivs(n) {
  var i;
  var img = document.getElementsByClassName("gallery-slides");
  var description = document.getElementsByClassName("gallery-slide-description");
  if (n > img.length) {
        slideIndex = 1
    }
  if (n < 1) {
        slideIndex = img.length
    }
  for (i = 0; i < img.length; i++) {
      img[i].style.display = "none";
      description[i].style.display = "none";
  }
    img[slideIndex - 1].style.display = "block";
    description[slideIndex - 1].style.display = "block";
}