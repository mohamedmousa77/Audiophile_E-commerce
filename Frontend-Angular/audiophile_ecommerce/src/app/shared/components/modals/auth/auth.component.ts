import { Component, ElementRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../../services/auth/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-auth',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.css'
})
export class AuthComponent {
  @ViewChild('authModal') authModal!: ElementRef;
  // @ViewChild('authModal') loginForm!: ElementRef;
  authForm: FormGroup;
  isLogin = true;
  isLoading = false;
  loginError: string | null = null;
  showPassword = false;

  constructor(
    private authService: AuthService,
    private fb: FormBuilder
  ) {
    this.authForm = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      phone: ['', Validators.required],
      address: [''],
      city: [''],
      country: [''],
      zipCode: ['']
    });
  }

  public showModal() {
    const modal = document.getElementById('authModal');
    if (modal) {
    modal.style.display = 'block';
    modal.classList.add('show');
    document.body.classList.add('auth-blur');
  }
  }

  toggleMode(event: Event) {
    event.preventDefault();
    this.isLogin = !this.isLogin;
    this.authForm.reset();
    this.loginError = null;

    if (this.isLogin) {
      this.authForm.get('fullName')?.clearValidators();
      this.authForm.get('phone')?.clearValidators();
    } else {
      this.authForm.get('fullName')?.setValidators(Validators.required);
      this.authForm.get('phone')?.setValidators(Validators.required);
    }

    this.authForm.get('fullName')?.updateValueAndValidity();
    this.authForm.get('phone')?.updateValueAndValidity();
  }


  onSubmit() {
    this.loginError = null;
    if (this.authForm.invalid) {
      console.log(`Auth form is not valid`);
      return;
    }

    if (this.isLogin) {
      this.login();
    } else {
      this.register();
    }
  }

  login() {
    this.isLoading = true;
    const { email, password } = this.authForm.value;

    this.authService.login(email, password).subscribe({
      next: (res) => {
        if (res.success && res.token) {
          this.authService.storeToken(res.token);
          this.closeModal();
          location.reload();
        } else {
          this.loginError = 'Login failed: invalid response.';
        }
      },
      error: (err) => {
        this.loginError = err.error?.message || 'Login failed. Please check credentials.';
        this.isLoading = false;
      }
    });
  }

  register(){
    this.authService.register(this.authForm.value).subscribe({
        next: res => {
          this.isLoading = false;
          this.loginError = null;
          this.closeModal();
          location.reload();
          alert('Registered successfully!');
        },
        error: err => {
          this.loginError = err.error?.message || 'Registration failed. Check your inputs.';
          this.isLoading = false;
        }
      });
  }

  closeModal() {
  const modal = document.getElementById('authModal');
  if (modal) {
    modal.style.display = 'none';
    modal.classList.remove('show');
    document.body.classList.remove('auth-blur');
  }
}
}
