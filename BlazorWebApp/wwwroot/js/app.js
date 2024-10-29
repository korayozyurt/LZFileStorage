function showLoading(message) {
    
    Swal.fire({
        title: 'Lütfen Bekleyiniz',
        html: message ? message : "İşleminiz yapılıyor. Lütfen bekleyiniz...",
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
}

function createLineChart(name, data, categories, divId) {
    document.getElementById(divId).replaceChildren()
    var options = {
        chart: {
            type: 'line',
            height: '400px',
        },
        series: [{
            name: 'boyut',
            data: data
        }],
        xaxis: {
            categories: categories
        }
    }

    var chart = new ApexCharts(document.getElementById(divId), options);

    chart.render();
}

function createBarChart(name, data, categories, divId, prefix) {
    document.getElementById(divId).replaceChildren()
    let chartData = [];
    for (let i = 0; i < data.length; i++) {
        chartData.push({ x: categories[i], y: data[i] });
    }
    var options = {
        chart: {
            type: 'bar',
            height: '400px',
        },
        plotOptions: {
            bar: {
                horizontal: true
            }
        },
        series: [{
            data: chartData
        }],
        dataLabels: {
            enabled: true,
            formatter: function (val) {
                return `${val} ${prefix}`; 
            }
        },
    }

    var chart = new ApexCharts(document.getElementById(divId), options);

    chart.render();
}