import { Empresa } from './../_models/Empresa';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class EmpresaService {

urlBase = 'https://localhost:5001/api/empresa';

constructor(private http: HttpClient) {}


getAllEmpresa(): Observable<Empresa[]>  {
    return this.http.get<Empresa[]>(this.urlBase);
  }

  getByIdEmpresa(id: number): Observable<Empresa> {
    return this.http.get<Empresa>(`${this.urlBase}/${id}`);
  }

  getByTemaEmpresa(nome: string): Observable<Empresa[]> {
    return this.http.get<Empresa[]>(`${this.urlBase}/getByNome/${nome}`);
  }

  postEmpresa(empresa: Empresa) {
    return this.http.post(this.urlBase, empresa);
  }

  putEmpresa(empresa: Empresa) {
    return this.http.put(`${this.urlBase}/${empresa.id}`, empresa);
  }

  deleteEmpresa(id: number) {
    return this.http.delete(`${this.urlBase}/${id}`);
  }

  uploadImage(file: File, name: string) {
    const fileToUpload = file[0] as File;
    const formData = new FormData();
    formData.append('file', fileToUpload, `${name}.jpg`);
    return this.http.post(`${this.urlBase}/uploadImage`, formData);
  }
}


