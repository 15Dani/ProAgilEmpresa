import { RedeSocial } from './RedeSocial';

export class Empresa {
    id: number;
    nome: string;
    dataCadastro: Date;
    descricao: string;
    qtdeFuncionarios: number;
    imagemURL: string;
    telefone: string;
    email: string;
    redesSociais: RedeSocial[];

}
