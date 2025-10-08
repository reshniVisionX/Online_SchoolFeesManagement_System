import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-excel-upload',
  imports: [CommonModule, FormsModule],
  templateUrl: './excel-upload.html',
  styleUrl: './excel-upload.css'
})
export class ExcelUpload {
  excelData = signal<any[]>([]);
courseId = signal<number | null>(null);
  private route = inject(ActivatedRoute);
  private http = inject(HttpClient);

  ngOnInit(): void {
    // Fetch the "id" from the URL and set it as courseId
   const id = Number(this.route.snapshot.paramMap.get('id'));
   this.courseId.set(id);
  }

  onFileChange(event: any) {
    const target: DataTransfer = <DataTransfer>(event.target);
    if (target.files.length !== 1) {
      console.error("Please select a single file");
      return;
    }

    const file = target.files[0];
    const reader: FileReader = new FileReader();

    reader.onload = (e: any) => {
      const bstr: string = e.target.result;
      const wb: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary' });

      const wsname: string = wb.SheetNames[0];
      const ws: XLSX.WorkSheet = wb.Sheets[wsname];

      const data = XLSX.utils.sheet_to_json(ws, { defval: '' });

      const mappedData = data.map((row: any) => ({
        sName: row['sName'] || '',
        sImage: row['sImage'] || '',
        dob: row['dob'] || '',
        bloodGrp: row['bloodGrp'] || '',
        parAddress: row['parAddress'] || '',
        parPhone: row['parPhone'] !== undefined && row['parPhone'] !== null
          ? String(row['parPhone']).trim()
          : '',
        parEmail: row['parEmail'] || '',
        category: row['category'] || '',
        isSports: row['isSports'] === 'TRUE',
        isMerit: row['isMerit'] === 'TRUE',
        isFG: row['isFG'] === 'TRUE',
        isWaiver: row['isWaiver'] === 'TRUE',
        roleId: 1,
        courseId: this.courseId()
      }));

      this.excelData.set(mappedData);
      console.log('Parsed Excel JSON (Signal):', this.excelData());
    };

    reader.readAsBinaryString(file);
  }

  // Update a specific field in the signal array
  updateSignal(index: number, key: string, value: any) {
    const currentData = [...this.excelData()];
    currentData[index][key] = value;
    this.excelData.set(currentData);
  }

  uploadJson() {
    const dataToSend = this.excelData();
    if (dataToSend.length === 0) {
      this.showToast("No data to upload", 'warn');
      return;
    }

    this.http.post('https://localhost:7126/api/Student/bulk-insert', dataToSend)
      .subscribe({
        next: (res) => {
          console.log('API Response:', res);
          this.showToast('Students uploaded successfully!', 'success');
        },
        error: (err) => {
          console.error('Upload Error:', err);
          let message = "Error uploading students";

          if (err.error) {
            if (err.error.message) message = err.error.message;
            else if (typeof err.error === 'string') message = err.error;
            else if (err.error.title) message = err.error.title;
          }

          this.showToast(message, 'error');
        }
      });
  }

  showToast(message: string, type: 'success' | 'error' | 'warn') {
    const toast = document.createElement('div');
    toast.textContent = message;
    toast.className = `custom-toast toast-${type}`;
    document.body.appendChild(toast);

    // Show in center of screen
    toast.style.position = 'fixed';
    toast.style.top = '50%';
    toast.style.left = '50%';
    toast.style.transform = 'translate(-50%, -50%)';
    toast.style.padding = '15px 25px';
    toast.style.borderRadius = '8px';
    toast.style.color = 'white';
    toast.style.fontWeight = 'bold';
    toast.style.fontSize = '16px';
    toast.style.zIndex = '10000';
    toast.style.textAlign = 'center';
    toast.style.boxShadow = '2px 4px 8px rgba(124, 122, 122, 0.93)';

    switch(type) {
      case 'success': toast.style.backgroundColor = '#73d176ff'; break;
      case 'error': toast.style.backgroundColor = '#fc9550ff'; break;
      case 'warn': toast.style.backgroundColor = '#def227ff'; break;
    }

    setTimeout(() => {
      toast.remove();
    }, 5000); 
  }
}
