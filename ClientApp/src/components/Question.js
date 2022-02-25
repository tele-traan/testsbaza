import { Component } from 'react'
import authService from './api-authorization/AuthorizeService'

export default class Question extends Component {
    constructor(props) {
        super(props);
        this.handleSubmit = this.handleSubmit.bind(this);
    }
    render() {
        return (<>
            <h3>Вопрос {this.props.number}</h3>
            <form>
                <input type="text" name="question" value={this.props.question} />
                <input type="text" name="answer" value={this.props.answer} />
                <input type="submit" onClick={e=>this.handleSubmit(e)} value="Сохранить" />
            </form>
            </>);
    }
    async handleSubmit(e) {
        e.preventDefault();
        const accessToken = await authService.getAccessToken();
        const response = await fetch('/api/test/change-test-question', {
            method: 'PUT',
            body: JSON.stringify({'questionId': this.props.questionId}),
            headers: !accessToken ? {} : { 'Authorization': `Bearer ${accessToken}` }
        });
        if (response.ok) {

        } else if (response.status === 401) {

        }
    }
}