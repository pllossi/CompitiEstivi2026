import { Article }           from '../domain/Article.js';
import { ArticleFirebaseRepository } from './ArticleFirebaseRepository.js';

export class ArticleService {

  constructor(repository = new ArticleFirebaseRepository()) {
    this._repository = repository;
  }


  async getAll() {
    const articles = await this._repository.getAll();
    return articles.sort((a, b) => (b.Timestamp ?? 0) - (a.Timestamp ?? 0));
  }

  async getById(id) {
    if (!id?.trim()) throw new Error('ID non valido');
    return this._repository.getById(id);
  }
  
  async getByTitle(title) {
    if (!title?.trim()) throw new Error('Titolo non valido');
    return this._repository.getByTitle(title);
  }
  
  async searchByContent(text) {
    if (!text?.trim()) return [];
    try {
      return await this._repository.getByContent(text);
    } catch (e) {
      return [];
    }
  }
  
  async searchByDate(dateString) {
    if (!dateString) return [];
    try {
      return await this._repository.getByDate(dateString);
    } catch (e) {
      return [];
    }
  }
  
  async searchByDateRange(startDateString, endDateString) {
    try {
      return await this._repository.getByDateRange(startDateString, endDateString);
    } catch (e) {
      return [];
    }
  }
  
  async save(article) {
    if (!article?.Title || !article.Title.trim()) throw new Error('Titolo non valido');
    return this._repository.save(article);
  }
  
  async delete(id) {
    if (!id?.trim()) throw new Error('ID non valido');
    return this._repository.delete(id);
  }
}
