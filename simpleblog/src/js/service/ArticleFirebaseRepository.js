import { IRepository } from '../repo/IRepository.js';
import { Article } from '../domain/Article.js';

const DB_BASE_URL = 'https://simpleblog-8a50b-default-rtdb.europe-west1.firebasedatabase.app';

export class ArticleFirebaseRepository extends IRepository {

  async getAll() {
    const res = await fetch(`${DB_BASE_URL}/articles.json`);
    if (!res.ok) throw new Error(`Errore HTTP ${res.status}`);
    const data = await res.json();
    if (!data) return [];
    return Object.values(data).map(raw => new Article(raw));
  }

  async getById(id) {
    const res = await fetch(`${DB_BASE_URL}/articles/${id}.json`);
    if (!res.ok) throw new Error(`Errore HTTP ${res.status}`);
    const data = await res.json();
    if (!data) throw new Error(`Articolo con id "${id}" non trovato`);
    return new Article(data);
  }

  async getByTitle(title) {
    const all = await this.getAll();
    const found = all.find(a => (a.Title ?? '').toLowerCase() === (title ?? '').toLowerCase());
    if (!found) throw new Error(`Articolo con titolo "${title}" non trovato`);
    return found;
  }

  async getByContent(content) {
    const all = await this.getAll();
    const q = (content ?? '').toLowerCase();
    return all.filter(a => (a.Content ?? '').toLowerCase().includes(q));
  }

  async getByDate(date) {
    if (!date) return [];
    const target = new Date(date);
    const all = await this.getAll();
    return all.filter(a => {
      if (!a.Timestamp) return false;
      const d = new Date(a.Timestamp * 1000);
      return d.getFullYear() === target.getFullYear()
        && d.getMonth() === target.getMonth()
        && d.getDate() === target.getDate();
    });
  }

  async getByDateRange(startDate, endDate) {
    const all = await this.getAll();
    const start = startDate ? new Date(startDate) : new Date(-8640000000000000);
    const end = endDate ? new Date(endDate) : new Date(8640000000000000);
    const results = all.filter(a => {
      if (!a.Timestamp) return false;
      const d = new Date(a.Timestamp * 1000);
      return d >= start && d <= end;
    });
    return results.sort((a, b) => (b.Timestamp ?? 0) - (a.Timestamp ?? 0));
  }

  async save(article) {
    if (!article) throw new Error('Articolo non valido');
    if (!article?.Title || !article.Title.trim()) throw new Error('Titolo non valido');
    if (!article?.Content || !article.Content.trim()) throw new Error('Contenuto non valido');
    if (!article.Id) article.Id = Article.generateId();
    if (!article.Timestamp) article.Timestamp = Math.floor(Date.now() / 1000);
    const res = await fetch(`${DB_BASE_URL}/articles/${article.Id}.json`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(article),
    });
    if (!res.ok) throw new Error(`Errore HTTP ${res.status}`);
  }

  async delete(id) {
    if (!id) throw new Error('ID non valido');
    if (!id?.trim()) throw new Error('ID non valido');
    const res = await fetch(`${DB_BASE_URL}/articles/${id}.json`, {
      method: 'DELETE',
    });
    if (!res.ok) throw new Error(`Errore HTTP ${res.status}`);
  }
}