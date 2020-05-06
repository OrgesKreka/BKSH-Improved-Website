$(function () {
  $("div.media-content p").flowtype({
    minimum: 500,
    minFont: 14,
    maxFont: 18,
    fontRatio: 30,
  });

  $(window)
    .resize(function () {
      var listOfInput = document.getElementsByClassName("has-tooltip-arrow");

      if ($(window).width() < 800)
        for (index = 0; index < listOfInput.length; index++) {
          listOfInput[index].classList.remove("has-tooltip-right");
          listOfInput[index].classList.add("has-tooltip-top");
        }
      else
        for (index = 0; index < listOfInput.length; index++) {
          listOfInput[index].classList.remove("has-tooltip-top");
          listOfInput[index].classList.add("has-tooltip-right");
        }
    })
    .trigger("resize");
});
