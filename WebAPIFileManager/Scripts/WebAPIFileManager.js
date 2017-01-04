var strBaseGeturl = "api/File/GetFilesJSON?dir=";
var strBaseUploadUrl = "api/File/UploadFile?dir=";
var strDefaultHomeDirectory = "/Files";

$(document).ready(function () {
    loadData();
    bindBtnUpLevel();
    bindBtnUpload();
});

function bindBtnUpload() {
    $('#upload').on('change', function (e) {
        var files = e.target.files;
        if (files.length > 0) {
            if (window.FormData !== undefined) {
                //get data from file
                var data = new FormData();
                for (var x = 0; x < files.length; x++) { data.append("file" + x, files[x]); }

                //upload file
                $.ajax({
                    type: "POST",
                    url: strBaseUploadUrl + location.hash.replace('#', '').replace(strBaseGeturl, ''), // Remove # and strBaseGeturl from location.hash
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (result) {
                        afterUpload(result);
                    },
                    error: function (request, status, x, z) {
                        var result = "Upload failure: " + " " + status + " " + x + " " + z;
                        if (request.responseText && request.responseText[0] == "{") result = JSON.parse(request.responseText).Message;
                        afterUpload(result);
                    }
                });
            }
        }
    });
}

function afterUpload(result) {
    location.reload();
    $('#uploadResult').text(result);
}

function bindBtnUpLevel() {
    $('#uplevel').click(function () {
        var dir = $('#directory').text().replace(/\/[^\/]+$/, ''); // Remove highest-level directory
        dir = (dir == "") ? strDefaultHomeDirectory : dir; // If dir is empty string then go to default home directory
        loadData(dir);
    });
}

function loadData(directory) {
    // Get directory string
    if (typeof directory == 'undefined') {
        var d = location.hash.replace('#', '').replace(strBaseGeturl, '');
         if (d !== '') {
             directory = d;
         } else {
             directory = strDefaultHomeDirectory;
         }
    }
        
    // Set location.hash
    location.hash = strBaseGeturl + directory; 

    //Update files table
    var filesTable = $('#files');    
    filesTable.find("tr:gt(0)").remove(); // Remove table rows (except header)    
    getFilesAndAppendTable(filesTable, directory);   // Get files and append table
}

function getFilesAndAppendTable(filesTable, directory) {
    $.getJSON(strBaseGeturl + directory, function (data) {
        for (var i = 0; i < data.Data.length; i++) {
            var name = (data.Data[i].name == null) ? "" : data.Data[i].name;
            var id = name.replace(/\ /g, '_').replace(/\./g, '_'); // Replace spaces and periods with underscores
            var type = (data.Data[i].type == null) ? "folder" : data.Data[i].type;
            var size = (data.Data[i].size == null) ? "" : data.Data[i].size;
            var count = (data.Data[i].count == null) ? "" : data.Data[i].count;
            var path = (data.Data[i].path == null) ? "" : data.Data[i].path;

            // Add table row
            filesTable.append(buildTableRow(id, name, type, size, count));

            // Add file action button
            addFileActionButton(type, id, path, name);
        }
        // Set directory text
        $('#directory').text(directory);
    });
}

function buildTableRow(id, name, type, size, count) {
    return "<tr><td id='td" + id + "'></td><td>&nbsp;" + name + "</td><td>" + type + "</td><td>" + size + "</td><td>" + count + "</td></tr>";
}

function addFileActionButton(type, id, path, name) {
    if (type == "folder") {
        // Add Navigate button
        $('<input></input>').attr({ 'type': 'button', 'id': id, 'data': path + name }).val("Navigate").appendTo($('#td' + id));;
        // Bind Navigate button click
        $('#' + id).click(function () { loadData($(this).attr('data')); });
    } else {
        // Add Download button
        $('<input></input>').attr({ 'type': 'button', 'id': id, 'data': path + name }).val("Download").appendTo($('#td' + id));
        // Bind Download button click
        $('#' + id).click(function () { window.open($(this).attr('data')); });
    }
}