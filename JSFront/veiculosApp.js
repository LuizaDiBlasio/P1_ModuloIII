// Variáveis globais
 const selectMarca = document.getElementById("fMarca")
 const selectModelo = document.getElementById("fModelo")
 const selectAno = document.getElementById("fAno")
 const selectVendido = document.getElementById("fVendido")
 const tabela = document.getElementById("tabela")
 const marca = document.getElementById("editMarca")
 const modelo = document.getElementById("editModelo")
 const inspecao = document.getElementById("editInspecao")
 const ano = document.getElementById("editAno")
 const vendido = document.getElementById("editVendido")
 const formVeiculo = document.getElementById("formVeiculo")
 let carIndex = document.getElementById("editIndex")
 let listaMarcasGlobal = [];
 let listaModelosGlobal = [];
 let listaAnosGlobal = [];
 const api = "https://localhost:44317";
 

//--------------------------------------------------
//Manipulação do DOM - FRONTEND

//Inicialização
async function Init(){

    try
    {
        atualizarInterfaceLogin()
    }
    catch (e)
    {
        document.getElementById('detail').style.display = 'block';
        document.querySelector('#detail-content h2').innerText = "Erro!";
        document.querySelector('#detail-content').innerHTML += `<p>${e.message}</p>`;
    }

    
    formVeiculo.addEventListener("submit", async function(e) {
    e.preventDefault(); 
        await Guardar(); 
        await PreencherLista();
    });

    window.addEventListener('beforeunload', function () {
    localStorage.clear();
});

}

//Preencher lista de carros
async function PreencherLista(){
    let carrosAPI = await obterCarros();
    MostrarTabela(carrosAPI)
}


async function FiltrarLista(){
    let valueAnos = selectAno.value
    let valueMarcas = selectMarca.value
    let valueModelo = selectModelo.value
    let valueVendido = selectVendido.value

    const filtro = {
        marca_Id : valueMarcas || null,
        modelo_Id : valueModelo || null,
        ano : valueAnos || null,
        vendido : valueVendido === ""? null : valueVendido === "true" 
    }

     const token = localStorage.getItem('token');

    try{
        const result = await fetch(api + "/filtro", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token 
            },
            body: JSON.stringify(filtro)
        });

        const listaResultados = await result.json()

        MostrarTabela(listaResultados)
    }
    catch(error){
        console.error("Erro ao filtrar:", error);
    }
    
}

//Preecher comboboxes
function PreencherCombos(lista, elementId, ordAsc){
     const p = document.getElementById(`${elementId}`);
    p.innerHTML = '';

    const option = document.createElement('option');
    option.value = '';

    if(elementId === "fMarca" || "selectMarca"){option.innerText = "Todas as marcas"}
    if(elementId === "fModelo" || "selectModelo"){option.innerText = "Todos os modelos"}

    p.appendChild(option);

    lista.sort();
    if (!ordAsc) lista.reverse();

    lista.forEach(item => {
        const option = document.createElement('option');
        option.value = item.marca_Id || item.modelo_Id || item.id; 
        option.innerText = item.nome || item.Nome; 
        
        option.value = item.marca_Id || item.modelo_Id || item.id; 
    option.innerText = item.nome || item.Nome;
        
        p.appendChild(option);
    });

}

function PreencherComboAno(lista, elementId, ordAsc){
     const p = document.getElementById(`${elementId}`);
    p.innerHTML = '';

    const option = document.createElement('option');
    option.value = '';
    option.innerText = "Todos os anos"
    p.appendChild(option);

    lista.sort((a, b) => ordAsc ? a.ano - b.ano : b.ano - a.ano);

    lista.forEach(item => {
        const option = document.createElement('option');
        const valorAno = item.ano !== undefined ? item.ano : item.Ano;
        
        option.value = valorAno; 
        option.innerText = valorAno;
        
        p.appendChild(option);
    });

}

function PreencherComboEditAno(){
   
    const selectAno = document.getElementById('editAno');
    selectAno.innerHTML = '';
    const anoAtual = new Date().getFullYear();
    const option = document.createElement('option');
    option.value = '';
    option.innerText = "Todos os anos"
    selectAno.appendChild(option);

    for(let i = 1900; i <= anoAtual; i++ ){
        const option = document.createElement('option');
        option.value = i; 
        option.innerText = i;
        
        selectAno.appendChild(option);
    };

}

/**
 * Método para avaliar estado da inspeção
 * @param {*} data - data da inspeção
 * @returns - string com html do estado da inspeção
 */
function inspecaoEstado(data) {
            const agora = new Date();
            const diffMeses = (agora - data) / (1000 * 60 * 60 * 24 * 30);
            if (diffMeses > 12) return '<span class="vendido">Expirada</span>';
            if (diffMeses > 10) return '<span class="aviso">A expirar</span>';
            return '<span class="ok">Válida</span>';
        } 

/**
 * Método para construir a tabela a partir de uma lista 
 * @param {Array} list 
 */
function MostrarTabela(list){ 

    tabela.innerHTML = ""; 

    list.forEach((item) => {
        // 1. Criar a linha
        let newRow = document.createElement(`tr`);

        // 3. Criar as células manualmente para garantir a ordem correta
        newRow.innerHTML = `
            <td>${item.nomeMarca}</td>
            <td>${item.nomeModelo}</td>
            <td>${item.ano}</td>
            <td>${ShowDate(item.ultimaInspec)} (${inspecaoEstado(new Date(item.ultimaInspec))})</td>
            <td class="${item.vendido ? 'estado-vendido' : 'estado-disponivel'}">
                ${item.vendido ? 'Vendido' : 'Disponível'}
            </td>
        `;

        // 4. Criar célula de Ações
        let actionsCell = document.createElement(`td`);

        // Botão Editar
        let btnEditar = document.createElement(`button`);
        btnEditar.textContent = "Editar";
        // Usamos o ID real que vem do banco (carro_Id)
        btnEditar.onclick = () => PreencherTxtEditar(item.carro_Id);
        actionsCell.appendChild(btnEditar);

        // Botão Deletar
        let btnDeletar = document.createElement(`button`);
        btnDeletar.textContent = "Deletar";
        btnDeletar.onclick = () => DeletarItem(item.carro_Id);
        actionsCell.appendChild(btnDeletar);

        newRow.appendChild(actionsCell);
        tabela.appendChild(newRow);
    });
}

/**
 * Método para preencher campos com informações do veiculo a ser editado
 * @param {Number} id - id correspondente ao veículo
 */
async function PreencherTxtEditar(id){

    var carro = await obterCarro(id);

    //preencher valores
    marca.value = carro.marca
    modelo.value = carro.modelo
    ano.value = carro.ano
    vendido.checked = carro.vendido;
    inspecao.value = toInputDateEdit(carro.ultimaInspec)
    
    carIndex.value = id
}

/***
 * Atualiza o front do login de acordo com resposta da BD
 */
function atualizarInterfaceLogin() {
    const token = localStorage.getItem('token');
    const user = localStorage.getItem('username');

    if (token) {
        document.getElementById('login-form').style.display = 'none';
        document.getElementById('user-logado').style.display = 'block';
        document.getElementById('nome_usuario').innerText = user;
    } else {
        document.getElementById('login-form').style.display = 'block';
        document.getElementById('user-logado').style.display = 'none';
    }
}

/**
 * Limpa campos de input da secção de edit/save 
 */
function LimparEdits(){

    formVeiculo.reset();
    
    document.getElementById("editIndex").value = ""
}


//----------------------------------------------
//MÉTODOS BACK END

//Chamadas à API--------------------------------

//Método para obter modelos
async function obterModelos() {

   const token = localStorage.getItem('token');

    try {
        const result = await fetch(api + "/modelos", {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token 
            }
        });

        if (!result.ok) {
            throw new Error(`Erro na API: ${result.status}`);
        }

        return await result.json();

    } catch (error) {
        alert("Falha na comunicação:", error.message);
        throw error; 
    }

}

//Método para obter carros
async function obterCarros() {
   
    const token = localStorage.getItem('token');

    try {
        const result = await fetch(api + "/carros", {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token 
            }
        });

        if (!result.ok) {
            throw new Error(`Erro na API: ${result.status}`);
        }

        return await result.json();

    } catch (error) {
        alert("Falha na comunicação:", error.message);
        throw error; 
    }
}

//Método para obter carro
async function obterCarro(id) {

    const token = localStorage.getItem('token');
    
    try {
        const result = await fetch(api + "/carros/" + id, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token 
            }
        });

        if (!result.ok) {
            throw new Error(`Erro na API: ${result.status}`);
        } 

        return await result.json();
        
    } catch (error) {
        alert(error)
    }
}

//Método para obter marcas
async function obterMarcas() {

    const token = localStorage.getItem('token');

   try {
         const result = await fetch(api + "/marcas", {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token 
            }
        });

        if (!result.ok) {
            throw new Error(`Erro na API: ${result.status}`);
        }

        return await result.json();

    } catch (error) {
        alert("Falha na comunicação:", error.message);
        throw error; 
    }
} 

//Método para obter anos 
async function obterAnos() {

    const token = localStorage.getItem('token');
   
    try {
        const result = await fetch(api + "/anos", {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token 
            }
        });

        if (!result.ok) {
            throw new Error(`Erro na API: ${result.status}`);
        }

        return await result.json();

    } catch (error) {
        alert("Falha na comunicação:", error.message);
        throw error; 
    }
} 

/**
 * Método genérico para adicionar objetos na base de dados
 * 
 */
async function Guardar()
{
    const idExistente = carIndex.value;

    const createCarroDTO = {
        marca_Id : parseInt(marca.value),
        modelo_Id : parseInt(modelo.value),
        ano : ano.value,
        vendido : vendido.checked,
        ultimaInspec : inspecao.value || null
    }

    if(idExistente === ""){
    await CreateObject(createCarroDTO)
}
else{
    await EditObject(createCarroDTO, idExistente)
}

LimparEdits();
     
}

/**
 * Método de login
 */
async function Login()
{
     const user = document.getElementById('login_user').value;
     const pass = document.getElementById('login_pass').value;

     try {
        const response = await fetch(api + "/login", {
            method: 'POST', 
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username: user, password: pass })
        });

        if (response.ok) {
            const data = await response.json();
            localStorage.setItem('token', data.token); // Guarda o token JWT
            localStorage.setItem('username', user);
            
            atualizarInterfaceLogin();

             //Preencher combos
            listaAnosGlobal = await obterAnos(); 
            listaMarcasGlobal = await obterMarcas(); 
            listaModelosGlobal = await obterModelos();

            PreencherComboAno(listaAnosGlobal, "fAno", true);
            PreencherCombos(listaMarcasGlobal, "fMarca", true); 
            PreencherCombos(listaModelosGlobal, "fModelo", true); 
            PreencherComboEditAno(); 
            PreencherCombos(listaMarcasGlobal, "editMarca", true); 
            PreencherCombos(listaModelosGlobal, "editModelo", true); 

            alert("Login efetuado com sucesso!");
            await PreencherLista(); 
        } else {
            alert("Usuário ou senha inválidos.");
        }

    } catch (error) {
        console.error("Erro na requisição:", error);
    }
}

/**Método de Logout
 * 
 */
function Logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    atualizarInterfaceLogin();
    location.reload(); 
}


/**
 * Método genérico para editar objetos em um array
 * @param {*} o - objeto a ser editado
 * @param {*} id - id do objeto
 */
async function EditObject(o, id){

    const token = localStorage.getItem('token');

        try {
        const response = await fetch(`${api}/carrosUpdate/${id}`, {
            method: 'PUT', // Define o verbo HTTP
            headers: {
                'Content-Type': 'application/json', // Avisa a API que esta enviando JSON
                'Authorization': 'Bearer ' + token 
            },
            body: JSON.stringify(o) // Transforma o objeto JS em texto JSON
        });

        if (!response.ok) {
            throw new Error("Erro ao salvar o carro no servidor.");
        }
        
        alert("Carro editado com sucesso!");

    } catch (error) {
        alert("Erro na requisição:", error);
    }
}

async function CreateObject(o){
    
    const token = localStorage.getItem('token');

    try {
        const response = await fetch(api + "/carros", {
            method: 'POST', // Define o verbo HTTP
            headers: {
                'Content-Type': 'application/json', // Avisa a API que esta enviando JSON
                'Authorization': 'Bearer ' + token 
            },
            body: JSON.stringify(o) // Transforma o objeto JS em texto JSON
        });

        if (!response.ok) {
            throw new Error("Erro ao salvar o carro no servidor.");
        }
        
        alert("Carro adicionado com sucesso!");

    } catch (error) {
        console.error("Erro na requisição:", error);
    }
}



/**
 * Método genérico para deletar elementos de um array guardado em localStorage
 * @param {*} id - id do elemento a ser deletado
 */
async function DeletarItem(id){

    const token = localStorage.getItem('token');
    
    try {
        const response = await fetch(`${api}/carros/${id}`, {
            method: 'DELETE', // Define o verbo HTTP
            headers: {
                'Content-Type': 'application/json', // Avisa a API que esta enviando JSON
                'Authorization': 'Bearer ' + token 
            }
        });

        if (!response.ok) {
            throw new Error("Erro ao deletar o carro no servidor.");
        }
         ~
        await PreencherLista();
        
        alert("Carro deletado com sucesso!");
    } catch (error) {
        alert("Erro na requisição:", error);
    }
} 

//Manipulação das listas-----------------------------------------------------------------


/**
 * Busca veículos de acordo com a disponibilidade 
 * @param {Boolean} bool - Booleana true para disponível e false para indisponível
 * @param {Array} lista - Array de veículos inde realizar a busca
 * @returns Lista de veículos disponíveis ou indisponíveis
 */
function GetDisponibilidade(bool, lista){

    let listaVendidos = [];
    let listaDisponiveis = [];

    if(!lista){
        return[]
    }

    for(let item of lista){
        
            item.vendido? listaVendidos.push(item) : listaDisponiveis.push(item)
    }  
    
    if(bool === "true"){
        return listaVendidos
    }
    else{
        return listaDisponiveis
    }
}



/**
 * Faz reset da base de dados para a composição original do array
 */
async function ResetDb(){

    const token = localStorage.getItem('token');

    try {
        LimparDb();

        const response = await fetch(api + "/carrosResetDB", {
            method: 'POST', // Define o verbo HTTP
            headers: {
                'Content-Type': 'application/json', // Avisa a API que esta enviando JSON
                'Authorization': 'Bearer ' + token 
            }
        });

        if (!response.ok) {
            throw new Error("Erro ao resetar o carro no servidor.");
        }
        
        await PreencherLista()

    } catch (error) {
        console.error("Erro na requisição:", error);
    }
    
    
}

/**
 * Limpa a base de dados
 */
async function LimparDb(){

    const token = localStorage.getItem('token');

    try {
        const response = await fetch(`${api}/carrosDeleteAll`, {
            method: 'DELETE', // Define o verbo HTTP
            headers: {
                'Content-Type': 'application/json', // Avisa a API que esta enviando JSON
                'Authorization': 'Bearer ' + token
            }
        });

        if (!response.ok) {
            throw new Error("Erro ao limpar base de dados no servidor.");
        }

    } catch (error) {
        alert("Erro na requisição:", error);
    }
    await PreencherLista();
}

//Funções auxiliares -----------------------------------------------------------

/**
 * Método para criar uma nova data corrigir o mês 0 
 * @param {*} y - ano
 * @param {*} m - mês
 * @param {*} d - dia
 * @returns 
 */
function setDate(y, m, d) {
            let tmp = new Date(y, m, d);
            tmp.setMonth(tmp.getMonth() - 1);
            return tmp;
        }             


/**
 * Converta data para formato string
 * @param {*} d - data de entrada
 * @returns - data no formato string aaaa/mm/dd
 */
function toInputDateEdit(d) {
    const dataObj = new Date(d);
    
    const anoFormatado = dataObj.getFullYear();
    const mesFormatado = String(dataObj.getMonth() + 1).padStart(2, '0');
    const diaFormatado = String(dataObj.getDate()).padStart(2, '0');

    // O input type="date" só "entende" este formato:
    return `${anoFormatado}-${mesFormatado}-${diaFormatado}`;
}       

/**
 * Converte data para formato string
 * @param {*} d - data de entrada
 * @returns - data no formato string dd/mm/aaaa
 */
function ShowDate(d) {
            const dataObj = new Date(d);
            const pad = n => String(n).padStart(2, "0");
            return `${pad(dataObj.getDate())}/${pad(dataObj.getMonth() + 1)}/${dataObj.getFullYear()}`;
        }





