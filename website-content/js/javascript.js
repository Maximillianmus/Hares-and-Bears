function clickSmallNavMenu() {
  var smallNavBar = document.getElementById("small-nav-bar");
    
  if(smallNavBar.className.indexOf("w3-show") != -1) {
      smallNavBar.className = smallNavBar.className.replace(" w3-show", "");
  }
  else {
      smallNavBar.className += " w3-show";
  }
}