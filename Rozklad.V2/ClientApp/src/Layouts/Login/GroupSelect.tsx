// @ts-ignore
import React from 'react';
import Autosuggest from 'react-autosuggest';


let groups: string[] = []


const getSuggestions = (valueInInput: string) => {
    const inputValue = valueInInput.trim().toLowerCase();
    const inputLength = inputValue.length;

    return inputLength === 0 ? [] : groups.filter(group =>
        group.toLowerCase().slice(0, inputLength) === inputValue
    );
};


const getSuggestionValue = (suggestion: string) => suggestion
// Use your imagination to render suggestions.
const renderSuggestion = (suggestion: string) => (<div>{suggestion}</div>)

export interface GroupSelectProps {
    groups: string[]
    onSubmit: Function,
    groupValue: string | null
}

class GroupSelect extends React.Component<GroupSelectProps> {
    constructor(props: GroupSelectProps) {
        super(props);
        this.onSubmit = props.onSubmit
        groups = props.groups
    }

    onSubmit: Function | null = null;
    state: { value: string, suggestions: string[] } = {
        value: '',
        suggestions: []
    };
    onChange = (event: any, shit: any) => {
        this.setState({
            value: shit.newValue
        });
    };


    onSuggestionsFetchRequested = (hz: any) => {
        this.setState({
            suggestions: getSuggestions(hz.value)
        });
    };

    // Autosuggest will call this function every time you need to clear suggestions.
    onSuggestionsClearRequested = () => {
        this.setState({
            suggestions: []
        });
    };

    render() {
        const {value, suggestions} = this.state as any;
        let groupValue = this.props.groupValue
        // Autosuggest will pass through all these props to the input.
        const inputProps = {
            placeholder: 'Оберіть групу',
            value,
            onChange: this.onChange
        };

        // Finally, render it!
        // @ts-ignore

        return (
            <div className="row">
                <div className="col">
                    <Autosuggest
                        suggestions={suggestions}
                        onSuggestionsFetchRequested={this.onSuggestionsFetchRequested}
                        onSuggestionsClearRequested={this.onSuggestionsClearRequested}
                        getSuggestionValue={getSuggestionValue}
                        renderSuggestion={renderSuggestion}
                        inputProps={inputProps}
                    />
                </div>
                <div className="col-xs-1 ">
                    <a className="btn btn-secondary" role="button" style={{display: "block"}}>
                        <span className="text-white text" style={{display: "block"}} onClick={() => {
                            this.onSubmit!(this.state.value)
                        }}>
                            Далі
                        </span>
                    </a>
                </div>
            </div>)


    }
}

// if group not exist, disable button 
export default GroupSelect