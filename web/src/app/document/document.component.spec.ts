import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentComponent } from './document.component';
import { MarkdownComponent } from 'ngx-markdown';
import { ScanResultComponent } from '../scan-result/scan-result.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('DocumentComponent', () => {
  let component: DocumentComponent;
  let fixture: ComponentFixture<DocumentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [ HttpClientTestingModule ],
      declarations: [DocumentComponent, MarkdownComponent, ScanResultComponent ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DocumentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
