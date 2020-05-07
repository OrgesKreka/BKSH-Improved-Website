const url = "http://bksh-improved-website.gear.host/api";

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

$(function () {
  $("#input-form").on("submit", processForm);
});

function resetTabs() {
  var tabs = document.getElementsByClassName("search-tab");
  var i;
  for (i = 0; i < tabs.length; i++) {
    var tab = tabs[i];
    if (tab.classList.contains("is-active")) {
      tab.classList.remove("is-active");
    }
  }
}

function switchTab(element) {
  resetTabs();
  element.classList.toggle("is-active");
}

function processForm(e) {
  if (e.preventDefault) e.preventDefault;

  var database = $("ul > li.is-active").first().attr("value");
  var title = $("#title-search").val();
  var author = $("#author-search").val();
  var text = $("#text-search").val();
  var keyWord = $("#keyword-search").val();
  var year = $("#year-search").val();

  var errorDiv = $("#error-message");
  if (!errorDiv.hasClass("error-notification"))
    errorDiv.addClass("error-notification");

  errorDiv.empty();

  var resultsDiv = $("#result");
  var header = $(".header");

  resultsDiv.empty();
  header.empty();

  var isFormEmpty = true;

  if (Boolean(title)) isFormEmpty = false;

  if (Boolean(author)) isFormEmpty = false;

  if (Boolean(text)) isFormEmpty = false;

  if (Boolean(keyWord)) isFormEmpty = false;

  if (Boolean(year)) isFormEmpty = false;

  if (isFormEmpty) {
    errorDiv.append(
      "<strong>" +
        "Please, fill at least one of the search fields." +
        "</strong>"
    );
    errorDiv.toggleClass("error-notification");
    return false;
  }

  var loader = $(".pageloader").first();
  loader.toggleClass("is-active");

  $.ajax({
    url: url + "/Book/GetSearchResults",
    type: "POST",
    data: {
      title: title,
      author: author,
      text: text,
      keyword: keyWord,
      year: year,
      database: database,
    },
    headers: {
      Accept: "application/json",
    },
    success: function (data, textStatus, xhr) {
      $.each(data, function (index, item) {
        var div =
          "<div class= 'column'>" +
          "<div class='card'>" +
          "<header class='card-header'>" +
          "<p class='card-header-title'>" +
          "<a onclick=\"showDetails('" +
          item.bookInfoUrl +
          "')\">" +
          "<b class='tag is-success'>" +
          (index + 1) +
          " </b> " +
          item.title +
          "</a>" +
          "</p>" +
          "</header>" +
          "<div class='card-content'>" +
          "<div class='content'>" +
          "<h4>" +
          (item.authors.length > 0 ? item.authors : "-") +
          "</h4>" +
          "</div>" +
          "</div>" +
          "</div>" +
          "</div>";
        resultsDiv.append(div);
      });

      header.append(
        "<h3 class='title is-3 is-spaced'>" + data.length + " Rezultate </h3>"
      );
      gotoHash("#content");
    },
    error: function (xhr, status, error) {
      if (xhr.status == 404 || xhr.status == 0) {
        errorDiv.append("<strong>" + "Couldn't reach API url" + "</strong>");
      } else {
        errorDiv.append("<strong>" + xhr.responseText + "</strong>");
      }

      errorDiv.toggleClass("error-notification");
    },
    complete: function () {
      loader.removeClass("is-active");
    },
  });

  return false; // to prevent the default form behavior
}

function gotoHash(hash) {
  $("html, body").animate({ scrollTop: $(hash).offset().top }, "slow");
}

function showDetails(infoUrl) {
  var errorDiv = $("#error-message");
  if (!errorDiv.hasClass("error-notification"))
    errorDiv.addClass("error-notification");

  errorDiv.empty();

  // Shfaq loading
  var loader = $(".pageloader").first();
  loader.toggleClass("is-active");

  var modal = $(".modal");

  $.ajax({
    url: url + "/Book/GetSearchDetails",
    type: "POST",
    data: JSON.stringify(infoUrl),
    contentType: "application/json",
    headers: {
      Accept: "application/json",
    },
    success: function (data, textStatus, xhr) {
      if (data.authors !== null) createHtmlListForAuthors(data.authors);

      if (data.exemplars !== null) createHtmlTableFromJson(data.exemplars);

      if (data.title != null) {
        $("#info-title").empty();
        $("#info-title").append(data.title);
      }

      if (data.isbn != null) {
        $("#info-isbn").empty();
        $("#info-isbn").append("ISBN: " + data.isbn);
      }

      // Krijon divin me te dhenat specifike
      createSpecificationsDivFromData(data);

      // Krijon badget me kohen e leximit ne footer
      createReadingTimeDivInfo(
        data.minEstimatedReadingTime,
        data.maxEstimatedReadingTime
      );

      modal.toggleClass("is-active");
    },
    error: function (xhr, status, error) {
      if (xhr.status == 404 || xhr.status == 0) {
        errorDiv.append("<strong>" + "Couldn't reach API url" + "</strong>");
      } else {
        errorDiv.append("<strong>" + xhr.responseText + "</strong>");
      }

      errorDiv.toggleClass("error-notification");
    },
    complete: function () {
      loader.removeClass("is-active");
    },
  });
}

function closeModalForm() {
  $(".modal").toggleClass("is-active");
}

function createReadingTimeDivInfo(
  minEstimatedReadingTime,
  maxEstimatedReadingTime
) {
  if (
    minEstimatedReadingTime === null ||
    maxEstimatedReadingTime === null ||
    minEstimatedReadingTime === 0 ||
    maxEstimatedReadingTime === 0
  )
    return;

  var div =
    "<div class= 'columns has-tooltip-success has-tooltip-multiline has-tooltip-arrow' data-tooltip='Llogaritja bëhet duke marrë mesataren e leximit për faqe. \n Referenca: https://capitalizemytitle.com/reading-time/100-pages/ '>" +
    "<div class='column'>" +
    "<button class='button is-text is-small is-centered'>" +
    "<span class='badge'> " +
    (parseFloat(minEstimatedReadingTime) / 60.0).toFixed(2) +
    " orë </span>" +
    "Koha minimale e leximit" +
    "</button>" +
    "</div>" +
    "<div class='column'>" +
    "<button class='button is-text is-small is-centered'>" +
    "<span class='badge'> " +
    (parseFloat(maxEstimatedReadingTime) / 60.0).toFixed(2) +
    " orë </span>" +
    "Koha maksimale e leximit" +
    "</button>" +
    "</div>";

  $(".modal-card-foot").empty();
  $(".modal-card-foot").append(div);
}

function createSpecificationsDivFromData(data) {
  // Pergatit divin per permbajtjen e modalit
  var cardContent = $("#collapsible-card-content");
  cardContent.empty();

  var specificInfoDivContent =
    "<div class='card-content is-collapsible is-hidden'>";

  if (data.publishingData != null) {
    specificInfoDivContent +=
      "<p><b>Botuar: </b>" + data.publishingData + "</p>";
  }

  if (data.numberOfPages != 0) {
    specificInfoDivContent +=
      "<p><b>Numri Faqeve: </b>" + data.numberOfPages + "</p>";
  }

  if (data.physicalData != null) {
    specificInfoDivContent +=
      "<p><b>Të dhëna fizike: </b>" + data.physicalData + "</p>";
  }

  if (data.notes != null) {
    specificInfoDivContent += "<p><b>Shënime: </b>" + data.notes + "</p>";
  }

  if (data.clasification != null) {
    specificInfoDivContent +=
      "<p><b>Klasifikimi: </b>" + data.clasification + "</p>";
  }

  if (data.serieTitle != null) {
    specificInfoDivContent +=
      "<p><b>Titull Serie: </b>" + data.serieTitle + "</p >";
  }

  if (data.keywords != null) {
    specificInfoDivContent += "<p> <b>Fjalët kyçe: </b>";

    data.keywords.forEach((element) => {
      specificInfoDivContent +=
        "<span class='tag is-link'>" + element + "</span> ";
    });

    specificInfoDivContent += "</p>";
  }

  specificInfoDivContent += "</div>";
  cardContent.append(specificInfoDivContent);
}

function createHtmlTableFromJson(jsonObject) {
  // EXTRACT VALUE FOR HTML HEADER.
  var col = [];
  for (var i = 0; i < jsonObject.length; i++) {
    for (var key in jsonObject[i]) {
      if (col.indexOf(key) === -1) {
        col.push(key);
      }
    }
  }

  var headerText = [
    "Numri i inventarit",
    "Gjendja",
    "Lloji i huazimit",
    "Numri i vendit",
  ];

  // CREATE DYNAMIC TABLE.
  var table = document.createElement("table");

  // add bulma class to table
  table.classList.add("table");

  // CREATE HTML TABLE HEADER ROW USING THE EXTRACTED HEADERS ABOVE.
  var tr = table.insertRow(-1);

  for (var i = 0; i < col.length; i++) {
    var th = document.createElement("th");
    th.innerHTML = headerText[i];
    tr.appendChild(th);
  }

  // ADD JSON DATA TO THE TABLE AS ROWS.
  for (var i = 0; i < jsonObject.length; i++) {
    tr = table.insertRow(-1);

    for (var j = 0; j < col.length; j++) {
      var tabCell = tr.insertCell(-1);
      tabCell.innerHTML = jsonObject[i][col[j]];
    }
  }

  // FINALLY ADD THE NEWLY CREATED TABLE WITH JSON DATA TO A CONTAINER.
  var divContainer = document.getElementById("table-container");
  divContainer.innerHTML = "";
  divContainer.appendChild(table);
}

function createHtmlListForAuthors(listOfAuthors) {
  $("div.list").empty();
  $("div.list").append('<li class="list-item is-active">Autorët:</li>');

  listOfAuthors.forEach((element) => {
    $("div.list").append("<li class='list-item'>" + element + "</li>");
  });
}

function toggleIsHidden() {
  $(".is-collapsible").toggleClass("is-hidden");
}
