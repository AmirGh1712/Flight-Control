// Initialize and add the map
// Map Variables.
let flightsDict = {};
let selectedRow = null;
let path = null;
const mymap = L.map('mapid').setView([8, 0], 2);
mymap.on('click', unChooseFlight);
function unChooseFlight() {
    selectedRow = null
    showDetails(null)
    if (path != null) {
        mymap.removeLayer(path)
        path = null
    }
}

function raiseError(message) {
    const h = document.getElementById("header")
    h.innerHTML = "<div id=\"error\" class=\"alert alert-warning alert-dismissible fade" +
        "show\" role=\"alert\"><strong  <strong>Oops!  </strong></strong> <strong id=\"message\">" + message +
        "</strong><button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">" +
        "<span aria-hidden=\"true\">&times;</span></button></div >" + h.innerHTML
}

const airplaneIcon = L.icon({
    iconUrl: 'img/airplane.png',
    iconSize: [40, 40], // size of the icon
    iconAnchor: [20, 20], // point of the icon which will correspond to marker's location
    popupAnchor: [-3, -76] // point from which the popup should open relative to the iconAnchor
});
const fullAirplaneIcon = L.icon({
    iconUrl: 'img/bold_airplane.png',
    iconSize: [40, 40], // size of the icon
    iconAnchor: [20, 20], // point of the icon which will correspond to marker's location
    popupAnchor: [-3, -76] // point from which the popup should open relative to the iconAnchor
});

// Create map.
L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token=pk.eyJ1IjoiYW1pcmdoMTEiLCJhIjoiY2thbDcwcnd5MG41YzMxbHNxZ2piaXVibCJ9.6oBTzgsH-91i8n8NKS3YaA', {
    maxZoom: 18,
    minZoom: 1,
    attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors, ' +
        '<a href="https://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, ' +
        'Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
    id: 'mapbox/streets-v11',
    tileSize: 512,
    zoomOffset: -1,
    accessToken: 'pk.eyJ1IjoiYW1pcmdoMTEiLCJhIjoiY2thbDcwcnd5MG41YzMxbHNxZ2piaXVibCJ9.6oBTzgsH-91i8n8NKS3YaA'
}).addTo(mymap);

let markers = []

function showOnMap(jsons) {
    for (let i = 0; i < markers.length; i++) {
        mymap.removeLayer(markers[i])
    }
    markers = []
    jsons.forEach((element) => {
        addMarker(element)
    })
}

function addMarker(flight) {
    let iconImg = airplaneIcon
    if (selectedRow != null) {
        if (selectedRow.cells[0].innerHTML == flight.flight_id) {
            iconImg = fullAirplaneIcon
        }
    }
    const point = [flight.latitude, flight.longitude]
    m = L.marker(point, { icon: iconImg }).addTo(mymap).on('click', function () {
        let row = getRow(flight.flight_id)
        rowClicked(row)
    });
    markers.push(m)
}

function getRow(flightId) {
    let table = document.getElementById("internal")
    for (let i = 0, row; row = table.rows[i]; i++) {
        if (row.cells[0].innerHTML == flightId) {
            return row
        }

    }
    table = document.getElementById("external")
    for (let i = 0, row; row = table.rows[i]; i++) {
        if (row.cells[0].innerHTML == flightId) {
            return row
        }

    }
}

//id: Flight
class Flight {
    flight_id;
    longitude;
    latitude;
    passengers;
    company_name;
    data_time;
    is_external;
    constructor(jsonData) {
        Object.assign(this, jsonData);
    }

}

//call post func  when uploading flightplan-Oren
function post(json) {
    
    let url =  "/api/FlightPlan";
    $.post(url, json);
}
//notify the server to delete the flight- call when the remove button is being clicked
function notifyDelete(flightID) {
    let url =  "/api/Flights/" + flightID;
    $.ajax({ url: url, type: 'DELETE' })
        .fail(function (data) {
            raiseError("Fail deleting from the server: " + data.status)
        })

}
function handleFlights(jsons) {
    // the json is an array of jsons
    showOnMap(jsons)
    let toRemove = {};
    Object.assign(toRemove, flightsDict);
    jsons.forEach((element) => {
        if (element.flight_id in flightsDict) {
            delete toRemove[element.flight_id];
            flightsDict[element.flight_id].latitude = element.latitude;
            flightsDict[element.flight_id].longitude = element.longitude;
            //change the longitude and latitude
        }
        else {
            flightsDict[element.flight_id] = new Flight(element);
            
            addFlights(element);
        }

    });
    Object.keys(toRemove).forEach((key) => {
        //remove the row from the list
        //if the flight is internal
        let table = document.getElementById("internal");
        if (flightsDict[key].is_external == true) {
            table = document.getElementById("external");
        }
       
        for (let i = 0, row; row = table.rows[i]; i++) {
            //iterate through rows
            //rows would be accessed using the "row" variable assigned in the for loop
            if (row.cells[0].innerHTML == flightsDict[key].flight_id) {
                //this is the row we should delete
                row.parentNode.removeChild(row);
                if (row == selectedRow) {
                    selectedRow = null;
                    showDetails(null);
                }
            }
            
        }
        delete flightsDict[key];
    });
}
class FlightPlan {
    passengers;
    company_name;
    initial_location;
    segments;

    constructor(jsonData) {

        Object.assign(this, JSON.parse(JSON.stringify(jsonData)));
    }
}

setInterval(() => { this.requestFlights(); }, 2000);

let showError = false;
let hostname = window.location.host;

function requestFlights() {
    console.log(flightsDict);
    let date = new Date();
    //var now_utc = date.toUTCString();

    let y = date.getUTCFullYear();
    let m = (date.getUTCMonth() + 1);
    if (m < 10) { m = "0" + m; }
    let d = date.getUTCDate();
    if (d < 10) { d = "0" + d; }
    let h = date.getUTCHours();
    if (h < 10) { h = "0" + h; }
    let min = date.getUTCMinutes();
    if (min < 10) { min = "0" + min; }
    let s = date.getUTCSeconds();
    if (s < 10) { s = "0" + s; }
    date = y + "-" + m + "-" + d + "T" + h + ":" + min + ":" + s + "Z";
    let url = "/api/Flights?relative_to=" + date +"&sync_all";
    hostname = window.location.host;
    //url = "http://" + hostname + url;
    //alert(url);
    get(url, handleFlights);

}
function get(url, toDo) {
    console.log("start request");
    //alert("http://" + hostname + url);
    $.getJSON(url, (json) => {
        console.log(json);
        console.log("end request");
        // arrray of jsons
        toDo(json);
        showError = true;
    })
        .fail(function (data) {
            raiseError("Fail recieving information from the server Error Code: " + data.status)
        })
}


function addFlight() {
    // check with y
    var flight = `{
        "flight_id": "G45",
        "longitude": 33.244,
        "latitude": 31.12,
        "passengers": 26,
        "company_name": "SwissAir",
        "date_time": "2020-12-26T23:56:21Z",
        "is_external": false
    }`;

    // when receiving a get we will add all the 
    addFlights(flight);
}
function addFlights(flight) {
    let data = flight;


    // Find a <table> element with id="myTable":
    //var table = document.getElementById("external");
    var table = document.getElementById("external");
    if (data.is_external == false) {
        table = document.getElementById("internal");
    }

    // Create an empty <tr> element and add it to the 1st position of the table:
    var row = table.insertRow();
    row.addEventListener('click', function () {
        rowClicked(this);
    });
    row.classList.add("clickable-row");

    // Insert new cells (<td> elements) at the 1st and 2nd position of the "new" <tr> element:
    row.insertCell().innerHTML = data.flight_id;
    row.insertCell().innerHTML = data.company_name;
    //row.insertCell().innerHTML = data.longitude + "\n" + data.latitude;
    row.insertCell().innerHTML = data.passengers;
    //internal
    if (data.is_external == false) {
        row.insertCell().innerHTML = `<button type="button" onclick="deleteRow(this,event)" class="btn btn-danger">X</button>`;
    }
}



function rowClicked(row) {
    if (selectedRow != null) {
        selectedRow.classList.remove("success");
    }
    selectedRow = row;
    selectedRow = row;
    row.classList.add("success");
    get("/api/FlightPlan/" + row.cells[0].innerHTML, showflightPlan)
}

function showflightPlan(flightPlan) {
    drawPath(flightPlan)
    showDetails(flightPlan)
}

function drawPath(flightPlan) {
    console.log(flightPlan)
    const latlngs = []
    latlngs.push([flightPlan.initial_location.latitude, flightPlan.initial_location.longitude])
    for (let i = 0; i < flightPlan.segments.length; i++) {
        latlngs.push([flightPlan.segments[i].latitude, flightPlan.segments[i].longitude])
    }
    if (path != null) {
        mymap.removeLayer(path)
    }
    path = L.polyline(latlngs, { color: 'black' }).addTo(mymap);
}


function deleteRow(btn,event) {
    
    
    var row = btn.parentNode.parentNode;
    row.parentNode.removeChild(row);
    if (row == selectedRow) {
        unChooseFlight()
    }
    //notify the server to delete the flight
    notifyDelete(row.cells[0].innerHTML);
    event.stopPropagation();
}

function showDetails(flightPlan) {
    
    if (flightPlan == null) {
        document.getElementById("details_id").innerHTML = "";
        document.getElementById("details_company").innerHTML = "";
        document.getElementById("details_passengers").innerHTML = "";
        document.getElementById("details_initial_location").innerHTML = "";
        document.getElementById("details_final_location").innerHTML = "";
        document.getElementById("details_initial_time").innerHTML = "";
        document.getElementById("details_final_time").innerHTML = "";
    } else {
        document.getElementById("details_id").innerHTML = "Flight ID: " + selectedRow.cells[0].innerHTML;
        document.getElementById("details_company").innerHTML = "Company Name: " + flightPlan.company_name;
        document.getElementById("details_passengers").innerHTML = "Passengers: " + flightPlan.passengers;
        const initialLong = flightPlan.initial_location.longitude
        const initialLat = flightPlan.initial_location.latitude
        document.getElementById("details_initial_location").innerHTML = "Intial Location:" + initialLong + ", " + initialLat
        const lastSegment = flightPlan.segments[flightPlan.segments.length - 1]
        const finalLong = lastSegment.longitude
        const finalLat = lastSegment.latitude
        document.getElementById("details_final_location").innerHTML = "Final Location:" + finalLong + ", " + finalLat
        const initialTime = new Date(flightPlan.initial_location.date_time)
        document.getElementById("details_initial_time").innerHTML = "Intial Time:" + initialTime.toString()
        let sum = 0
        for (let i = 0; i < flightPlan.segments.length; i++) {
            sum += flightPlan.segments[i].timespan_seconds
        }
        const finalTime = new Date(flightPlan.initial_location.date_time)
        finalTime.setTime(finalTime.getTime() + sum * 1000)
        document.getElementById("details_final_time").innerHTML = "Final Time:" + finalTime.toString()
    }
}



/**
* Template Name: Vlava - v2.0.0
* Template URL: https://bootstrapmade.com/vlava-free-bootstrap-one-page-template/
* Author: BootstrapMade.com
* License: https://bootstrapmade.com/license/
*/
!(function($) {
  "use strict";

  // Smooth scroll for the navigation menu and links with .scrollto classes
  $(document).on('click', '.nav-menu a, .mobile-nav a, .scrollto', function(e) {
    if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') && location.hostname == this.hostname) {
      e.preventDefault();
      var target = $(this.hash);
      if (target.length) {

        var scrollto = target.offset().top;
        var scrolled = 20;

        if ($('#header').length) {
          scrollto -= $('#header').outerHeight()

          if (!$('#header').hasClass('header-scrolled')) {
            scrollto += scrolled;
          }
        }

        if ($(this).attr("href") == '#header') {
          scrollto = 0;
        }

        $('html, body').animate({
          scrollTop: scrollto
        }, 1500, 'easeInOutExpo');

        if ($(this).parents('.nav-menu, .mobile-nav').length) {
          $('.nav-menu .active, .mobile-nav .active').removeClass('active');
          $(this).closest('li').addClass('active');
        }

        if ($('body').hasClass('mobile-nav-active')) {
          $('body').removeClass('mobile-nav-active');
          $('.mobile-nav-toggle i').toggleClass('icofont-navigation-menu icofont-close');
          $('.mobile-nav-overly').fadeOut();
        }
        return false;
      }
    }
  });

  // Mobile Navigation
  if ($('.nav-menu').length) {
    var $mobile_nav = $('.nav-menu').clone().prop({
      class: 'mobile-nav d-lg-none'
    });
    $('body').append($mobile_nav);
    $('body').prepend('<button type="button" class="mobile-nav-toggle d-lg-none"><i class="icofont-navigation-menu"></i></button>');
    $('body').append('<div class="mobile-nav-overly"></div>');

    $(document).on('click', '.mobile-nav-toggle', function(e) {
      $('body').toggleClass('mobile-nav-active');
      $('.mobile-nav-toggle i').toggleClass('icofont-navigation-menu icofont-close');
      $('.mobile-nav-overly').toggle();
    });

    $(document).on('click', '.mobile-nav .drop-down > a', function(e) {
      e.preventDefault();
      $(this).next().slideToggle(300);
      $(this).parent().toggleClass('active');
    });

    $(document).click(function(e) {
      var container = $(".mobile-nav, .mobile-nav-toggle");
      if (!container.is(e.target) && container.has(e.target).length === 0) {
        if ($('body').hasClass('mobile-nav-active')) {
          $('body').removeClass('mobile-nav-active');
          $('.mobile-nav-toggle i').toggleClass('icofont-navigation-menu icofont-close');
          $('.mobile-nav-overly').fadeOut();
        }
      }
    });
  } else if ($(".mobile-nav, .mobile-nav-toggle").length) {
    $(".mobile-nav, .mobile-nav-toggle").hide();
  }

  // Navigation active state on scroll
  var nav_sections = $('section');
  var main_nav = $('.nav-menu, #mobile-nav');

  $(window).on('scroll', function() {
    var cur_pos = $(this).scrollTop() + 80;

    nav_sections.each(function() {
      var top = $(this).offset().top,
        bottom = top + $(this).outerHeight();

      if (cur_pos >= top && cur_pos <= bottom) {
        if (cur_pos <= bottom) {
          main_nav.find('li').removeClass('active');
        }
        main_nav.find('a[href="#' + $(this).attr('id') + '"]').parent('li').addClass('active');
      }
    });
  });

  // Toggle .header-scrolled class to #header when page is scrolled
  $(window).scroll(function() {
    if ($(this).scrollTop() > 100) {
      $('#header').addClass('header-scrolled');
    } else {
      $('#header').removeClass('header-scrolled');
    }
  });

  if ($(window).scrollTop() > 100) {
    $('#header').addClass('header-scrolled');
  }

  // Back to top button
  $(window).scroll(function() {
    if ($(this).scrollTop() > 100) {
      $('.back-to-top').fadeIn('slow');
    } else {
      $('.back-to-top').fadeOut('slow');
    }
  });

  $('.back-to-top').click(function() {
    $('html, body').animate({
      scrollTop: 0
    }, 1500, 'easeInOutExpo');
    return false;
  });

  // Testimonials carousel (uses the Owl Carousel library)
  $(".testimonials-carousel").owlCarousel({
    autoplay: true,
    dots: true,
    loop: true,
    items: 1
  });

  // Porfolio isotope and filter
  $(window).on('load', function() {
    var portfolioIsotope = $('.portfolio-container').isotope({
      itemSelector: '.portfolio-item',
      layoutMode: 'fitRows'
    });

    $('#portfolio-flters li').on('click', function() {
      $("#portfolio-flters li").removeClass('filter-active');
      $(this).addClass('filter-active');

      portfolioIsotope.isotope({
        filter: $(this).data('filter')
      });
    });

    // Initiate venobox (lightbox feature used in portofilo)
    $(document).ready(function() {
      $('.venobox').venobox();
    });
  });

  // Portfolio details carousel
  $(".portfolio-details-carousel").owlCarousel({
    autoplay: true,
    dots: true,
    loop: true,
    items: 1
  });

})(jQuery);




let dropArea = document.getElementById('drop-area')

    ;['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
        dropArea.addEventListener(eventName, preventDefaults, false)
    })

    ;['dragenter', 'dragover'].forEach(eventName => {
        dropArea.addEventListener(eventName, highlight, false)
    })

    ;['dragleave', 'drop'].forEach(eventName => {
        dropArea.addEventListener(eventName, unhighlight, false)
    })


function highlight(e) {
    dropArea.classList.add('highlight')
}

function unhighlight(e) {
    dropArea.classList.remove('highlight')
}

function preventDefaults(e) {
    e.preventDefault()
    e.stopPropagation()
}

dropArea.addEventListener('drop', handleDrop, false)

function handleDrop(e) {
    let dt = e.dataTransfer
    let files = dt.files

    handleFiles(files)
}


function handleFiles(files) {
    files = [...files]
    files.forEach(uploadFile)
   
}

function uploadFile(file) {
    
    
   
    let reader = new FileReader()
    reader.onload = function () {
        // The file's text will be printed here
        //document.write(e.target.result)
        $.ajax({
            url: "/api/FlightPlan",
            type: 'POST',
            data: reader.result,
            contentType: "application/json",

            success: function (data) {
                //flightsArray = JSON.parse(data);
                //consloe
            },
            error: function (xhr) {
                raiseError("Failed to post flightPlan to the server! Status Code: " + xhr.status)
                //ErrorHandler.showError("failed to post flightPlan to the server!");
            }  // tell jQuery not to set contentType

        });
    };
    reader.readAsText(file);   
}