charts = {};

function createCharts() {
    var optionsBarChart = {
        series: [{
            type: 'bar',
            xKey: 'item1',
            yKeys: ['item2'],
        }],
        axes: [
            {
                type: 'number',
                position: 'bottom',
                tick: {
                    count: 4,
                },
            },
            {
                type: 'category',
                position: 'left',
                line: {
                    width: 0,
                },
                tick: {
                    width: 0,
                }
            },
        ],
        legend: {
            enabled: false
        },
        title: {
            text: ''
        },
        subtitle: {
            text: ''
        },
    };

    charts['unidadesPorCategoria'] = agCharts.AgChart.create(optionsBarChart);
    charts['unidadesPorCategoria'].container = document.querySelector('#unidadesPorCategoria');
    charts['unidadesPorCategoria'].title.text = 'Unidades Vendidas por Categoría';
    charts['unidadesPorCategoria'].series[0].yNames = ['Unidades Vendidas'];

    charts['importePorCategoria'] = agCharts.AgChart.create(optionsBarChart);
    charts['importePorCategoria'].container = document.querySelector('#importePorCategoria');
    charts['importePorCategoria'].title.text = 'Importe por Categoría';
    charts['importePorCategoria'].subtitle.text = 'Millones EUR';
    charts['importePorCategoria'].series[0].yNames = ['Importe'];
    charts['importePorCategoria'].axes[0].label.formatter = (params) => { return (params.value / 1000000).toFixed() };

    charts['beneficioPorCategoria'] = agCharts.AgChart.create(optionsBarChart);
    charts['beneficioPorCategoria'].container = document.querySelector('#beneficioPorCategoria');
    charts['beneficioPorCategoria'].title.text = '% Beneficio por Categoría';
    charts['beneficioPorCategoria'].series[0].yNames = ['% Beneficio'];
    charts['beneficioPorCategoria'].axes[0].label.formatter = (params) => { return (params.value * 100).toFixed(0) };

    var optionsLineChart = {
        series: [{
            xKey: 'item1',
            yKey: 'item2',
        }],
        axes: [
            {
                type: 'category',
                position: 'bottom'
            },
            {
                type: 'number',
                position: 'left',
                min: 0,
            }
        ],
        legend: {
            enabled: false
        },
        title: {
            text: ''
        },
        subtitle: {
            text: ''
        }
    };

    charts['unidadesPorMes'] = agCharts.AgChart.create(optionsLineChart);
    charts['unidadesPorMes'].container = document.querySelector('#unidadesPorMes');
    charts['unidadesPorMes'].title.text = 'Unidades Vendidas por Mes';
    charts['unidadesPorMes'].series[0].yName = 'Unidades Vendidas';

    charts['importePorMes'] = agCharts.AgChart.create(optionsLineChart);
    charts['importePorMes'].container = document.querySelector('#importePorMes');
    charts['importePorMes'].title.text = 'Importe por Mes';
    charts['importePorMes'].series[0].yName = 'Importe';
    charts['importePorMes'].subtitle.text = 'Miles EUR';
    charts['importePorMes'].axes[1].label.formatter = (params) => { return (params.value / 1000).toFixed() };

    charts['beneficioPorMes'] = agCharts.AgChart.create(optionsLineChart);
    charts['beneficioPorMes'].container = document.querySelector('#beneficioPorMes');
    charts['beneficioPorMes'].title.text = '% Beneficio por Mes';
    charts['beneficioPorMes'].series[0].yName = '% Beneficio';
    charts['beneficioPorMes'].axes[1].label.formatter = (params) => { return (params.value * 100).toFixed(0) };
}


function updateCharts(data) {
    charts['unidadesPorCategoria'].data = data['unidadesPorCategoria'];
    charts['importePorCategoria'].data = data['importePorCategoria'];
    charts['beneficioPorCategoria'].data = data['beneficioPorCategoria'];

    charts['unidadesPorMes'].data = data['unidadesPorMes'];
    charts['importePorMes'].data = data['importePorMes'];
    charts['beneficioPorMes'].data = data['beneficioPorMes'];
}

function updateTexts(data) {
    document.querySelector('#unidadesVendidas').innerHTML = formatNumber(data.unidadesVendidas);
    document.querySelector('#importe').innerHTML = formatNumber((data.importe).toFixed(0)) + ' EUR';
    document.querySelector('#beneficio').innerHTML = formatNumber((data.beneficio * 100).toFixed(2)) + ' %';
}

function formatNumber(num) {
    return num.toString().replace('.',',').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.')
}

function getData(yearElement) {
    var selectedYears = yearElement.selectedOptions;

    if (selectedYears.length <= 0) {
        return;
    }

    var year = selectedYears[0].value;

    var xhr = new XMLHttpRequest();
    xhr.open("POST", 'salesReport/data?year=' + year, true);
    xhr.setRequestHeader('Content-type', 'application/json; charset=utf-8');
    xhr.onload = function () {
        var data = JSON.parse(xhr.responseText);
        if (xhr.readyState == 4 && xhr.status == "200") {
            console.table(data);
            updateTexts(data);
            updateCharts(data);
        } else {
            console.error(data);
        }
    }
    xhr.send();
}

document.addEventListener('DOMContentLoaded', function () {
    var yearElement = document.getElementById('Year');

    createCharts();
    getData(yearElement);

    yearElement.addEventListener('change', (event) => {
        getData(yearElement);
    });
});
