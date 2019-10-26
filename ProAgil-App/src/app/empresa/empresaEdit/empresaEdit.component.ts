import { Component, OnInit } from '@angular/core';
import { FormGroup, FormArray, FormBuilder, Validators } from '@angular/forms';
import { Empresa } from 'src/app/_models/Empresa';
import { EmpresaService } from 'src/app/_services/empresa.service';
import { BsLocaleService } from 'ngx-bootstrap/datepicker/public_api';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute } from '@angular/router';

@Component({
  // tslint:disable-next-line: component-selector
  selector: 'app-empresaEdit',
  templateUrl: './empresaEdit.component.html',
  styleUrls: ['./empresaEdit.component.css']
})
export class EmpresaEditComponent implements OnInit {
  titulo = 'Edição do Cadastro';
  registerForm: FormGroup;
  empresa: Empresa = new Empresa();
  imagemURL = 'assets/img/upload.png';
  fileNameToUpdate: string;
  file: File;
  dataAtual = '';
  get redesSociais(): FormArray {
    return this.registerForm.get('redesSociais') as FormArray;
  }
  constructor(private empresaService: EmpresaService,

              private formBuilder: FormBuilder,
              private localeService: BsLocaleService,
              private toastr: ToastrService,
              private router: ActivatedRoute) {
      this.localeService.use('pt-br');
    }
    ngOnInit() {
      this.validation();
      this.carregarEmpresa();
    }
    carregarEmpresa() {
      const idEmpresa = +this.router.snapshot.paramMap.get('id');
      this.empresaService.getByIdEmpresa(idEmpresa)
      .subscribe(
        (empresa: Empresa) => {
          this.empresa = Object.assign({}, empresa);
          this.fileNameToUpdate = empresa.imagemURL.toString();
          this.imagemURL = `https://localhost:5000/Resources/images/${this.empresa.imagemURL}?_ts=${this.dataAtual}`;
          this.empresa.imagemURL = '';
          this.registerForm.patchValue(this.empresa);
        });
      this.empresa.redesSociais.forEach( redeSocial => {
          this.redesSociais.push(this.criaRedeSocial(redeSocial));
        });
      }
     onFileChange(empresa: any, file: FileList) {
      const reader = new FileReader();
      reader.onload = (event: any) => this.imagemURL = event.target.result;
      this.file = empresa.target.files;
      reader.readAsDataURL(file[0]);
    }

validation() {
      this.registerForm = this.formBuilder.group({
        id: [],
        nome: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(50)]],
        descricao: ['', Validators.required],
        dataCadastro: ['', Validators.required],
        telefone: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        qtdeFuncionarios: ['', [Validators.required, Validators.max(120000)]],
        imagemURL: [''],
        redesSociais: this.formBuilder.array([])
      });
    }
   criaRedeSocial(redeSocial: any): FormGroup {
      return this.formBuilder.group({
        id: [redeSocial.id],
        nome: [redeSocial.nome, Validators.required],
        url: [redeSocial.url, Validators.required]
      });
    }
adicionarRedeSocial() {
      this.redesSociais.push(this.criaRedeSocial({id: 0}));
    }

removerRedeSocial(id: number) {
      this.redesSociais.removeAt(id);
    }
salvarEmpresa() {
      if (this.registerForm.valid) {
        this.empresa = Object.assign({id: this.empresa.id}, this.registerForm.value);
        this.empresa.imagemURL = this.fileNameToUpdate;
        this.empresaService.putEmpresa(this.empresa).subscribe(
          (empresa: Empresa) => {
            if (this.registerForm.get('imagemURL').value !== '') {
              this.empresaService.uploadImage(this.file, empresa.id.toString()).subscribe(
                () => {
                  this.dataAtual = new Date().getMilliseconds().toString();
                  this.imagemURL = `https://localhost:5000/Resources/images/${this.empresa.imagemURL}?_ts=${this.dataAtual}`;

                  this.toastr.success('Empresa salvo com sucesso!');
                },
                error => {
                  this.toastr.error(`Ocorreu um erro ao tentar salvar empresa: ${error.error}`);
                }
                );
              } else {
                this.toastr.success('Empresa salvo com sucesso!');
              }
            },
            error => {
              this.toastr.error(`Ocorreu um erro ao tentar salvar empresa: ${error.error}`);
            }
            );
          }
        }
      }